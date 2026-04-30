using System.Text;

namespace MementoEditorDemo.Models;

public sealed class EditorMemento
{
    // ── Snapshot alanları ──────────────────────────────────────────
    public string Content { get; }
    public int CursorPosition { get; }
    public DateTime CreatedAt { get; }

    // ── Delta alanları (karşılaştırma için) ───────────────────────
    public int DeltaStart { get; }
    public string DeltaRemoved { get; }
    public string DeltaAdded { get; }

    // ── Boyutlar ──────────────────────────────────────────────────
    /// Snapshot modu: tüm içeriğin bayt boyutu + sabit overhead
    public int SnapshotBytes =>
        Encoding.UTF8.GetByteCount(Content) + 32;

    /// Delta modu: yalnızca değişen kısımların boyutu + sabit overhead
    public int DeltaBytes =>
        sizeof(int)
        + Encoding.UTF8.GetByteCount(DeltaRemoved)
        + Encoding.UTF8.GetByteCount(DeltaAdded)
        + 32;

    public string Preview =>
        Content.Length == 0 ? "(boş)"
        : Content.Length <= 28 ? Content
        : Content[..28] + "…";

    internal EditorMemento(string content, int cursorPosition,
                            string previousContent)
    {
        Content        = content;
        CursorPosition = cursorPosition;
        CreatedAt      = DateTime.Now;

        var (start, removed, added) = ComputeDelta(previousContent, content);
        DeltaStart   = start;
        DeltaRemoved = removed;
        DeltaAdded   = added;
    }

    // ── Serialization (Export/Import için) ────────────────────────
    internal EditorMemento(string content, int cursorPosition, DateTime createdAt)
    {
        Content        = content;
        CursorPosition = cursorPosition;
        CreatedAt      = createdAt;
        DeltaRemoved   = string.Empty;
        DeltaAdded     = string.Empty;
    }

    // ── Delta hesaplama: ortak prefix/suffix sıkıştırması ─────────
    private static (int start, string removed, string added)
        ComputeDelta(string oldText, string newText)
    {
        int start = 0;
        while (start < oldText.Length && start < newText.Length
               && oldText[start] == newText[start])
            start++;

        int endOld = oldText.Length  - 1;
        int endNew = newText.Length  - 1;
        while (endOld >= start && endNew >= start
               && oldText[endOld] == newText[endNew])
        { endOld--; endNew--; }

        string removed = endOld >= start ? oldText[start..(endOld + 1)] : "";
        string added   = endNew >= start ? newText[start..(endNew + 1)] : "";
        return (start, removed, added);
    }
}
