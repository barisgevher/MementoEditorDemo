namespace MementoEditorDemo.Models;

public class TextEditor
{
    public string Content { get; set; } = string.Empty;
    public int CursorPosition { get; set; }

    public EditorMemento Save() => new(Content, CursorPosition, Content);

    public void Restore(EditorMemento memento)
    {
        Content        = memento.Content;
        CursorPosition = memento.CursorPosition;
    }
}
