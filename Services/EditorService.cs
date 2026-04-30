using System.Text.Json;
using MementoEditorDemo.Models;

namespace MementoEditorDemo.Services;

public class EditorService
{
    private readonly TextEditor    _editor  = new();
    private readonly EditorHistory _history = new();

    public SaveMode Mode { get; private set; } = SaveMode.Snapshot;

    public string  Content    => _editor.Content;
    public bool    CanUndo    => _history.CanUndo;
    public bool    CanRedo    => _history.CanRedo;
    public int     UndoCount  => _history.UndoCount;
    public int     RedoCount  => _history.RedoCount;

    public IEnumerable<EditorMemento> UndoHistory => _history.UndoHistory;

    public int TotalSnapshotBytes =>
        _history.UndoHistory.Sum(m => m.SnapshotBytes);

    public int TotalDeltaBytes =>
        _history.UndoHistory.Sum(m => m.DeltaBytes);

    public void UpdateContent(string newContent)
    {
        var before = new EditorMemento(
            _editor.Content,
            _editor.CursorPosition,
            _editor.Content);
        _history.Push(before);
        _editor.Content = newContent;
    }

    public void Undo()
    {
        var current = new EditorMemento(
            _editor.Content, _editor.CursorPosition, _editor.Content);
        var previous = _history.PopUndo(current);
        if (previous is not null) _editor.Restore(previous);
    }

    public void Redo()
    {
        var current = new EditorMemento(
            _editor.Content, _editor.CursorPosition, _editor.Content);
        var next = _history.PopRedo(current);
        if (next is not null) _editor.Restore(next);
    }

    public void SetMode(SaveMode mode) => Mode = mode;

    public void Clear()
    {
        _editor.Content = string.Empty;
        _history.Clear();
    }

    public string ExportJson()
    {
        var items = _history.ToList().Select(m => new MementoDto
        {
            Content        = m.Content,
            CursorPosition = m.CursorPosition,
            CreatedAt      = m.CreatedAt
        }).ToList();

        return JsonSerializer.Serialize(new ExportDto
        {
            CurrentContent = _editor.Content,
            History        = items
        }, new JsonSerializerOptions { WriteIndented = true });
    }

    public bool ImportJson(string json)
    {
        try
        {
            var dto = JsonSerializer.Deserialize<ExportDto>(json);
            if (dto is null) return false;

            _editor.Content = dto.CurrentContent ?? string.Empty;
            var mementos = dto.History?
                .Select(d => new EditorMemento(
                    d.Content ?? string.Empty,
                    d.CursorPosition,
                    d.CreatedAt))
                ?? Enumerable.Empty<EditorMemento>();

            _history.LoadFrom(mementos);
            return true;
        }
        catch { return false; }
    }

    private class ExportDto
    {
        public string?           CurrentContent { get; set; }
        public List<MementoDto>? History        { get; set; }
    }

    private class MementoDto
    {
        public string?  Content        { get; set; }
        public int      CursorPosition { get; set; }
        public DateTime CreatedAt      { get; set; }
    }
}
