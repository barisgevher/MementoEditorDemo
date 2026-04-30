namespace MementoEditorDemo.Models;

public class EditorHistory
{
    private readonly Stack<EditorMemento> _undoStack = new();
    private readonly Stack<EditorMemento> _redoStack = new();

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;
    public int  UndoCount => _undoStack.Count;
    public int  RedoCount => _redoStack.Count;

    public IEnumerable<EditorMemento> UndoHistory => _undoStack;

    public void Push(EditorMemento memento)
    {
        _undoStack.Push(memento);
        _redoStack.Clear();
    }

    public EditorMemento? PopUndo(EditorMemento current)
    {
        if (!CanUndo) return null;
        _redoStack.Push(current);
        return _undoStack.Pop();
    }

    public EditorMemento? PopRedo(EditorMemento current)
    {
        if (!CanRedo) return null;
        _undoStack.Push(current);
        return _redoStack.Pop();
    }

    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
    }

    /// Export için sıralı liste (en eskiden en yeniye)
    public List<EditorMemento> ToList() =>
        _undoStack.Reverse().ToList();

    /// Import: listeyi stack'e yükle
    public void LoadFrom(IEnumerable<EditorMemento> items)
    {
        _undoStack.Clear();
        _redoStack.Clear();
        foreach (var m in items)
            _undoStack.Push(m);
    }
}
