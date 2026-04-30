# Memento Tasarım Deseni — Blazor WebAssembly Demo

Memento (GoF #18) desenini gerçek bir metin editörü üzerinde gösteren, tarayıcıda çalışan (client-side) Blazor WASM uygulaması.

---

## Özellikler

| Özellik                              | Açıklama                                                             |
| ------------------------------------ | -------------------------------------------------------------------- |
| **Word benzeri Undo/Redo**           | Her tuş vuruşu bir Memento üretir, iki yönlü Stack ile geri/ileri al |
| **Caretaker Stack Görselleştirmesi** | Memento nesnelerinin Stack'e girip çıkışı animasyonlu izlenir        |
| **Snapshot vs Delta Modu**           | İki kaydetme stratejisinin bellek maliyeti canlı karşılaştırılır     |
| **Bellek X-Ray**                     | Her Memento'nun bayt boyutu ve tasarruf yüzdesi anlık gösterilir     |
| **Export / Import**                  | Tüm geçmiş JSON olarak dışa aktarılır, sonra geri yüklenir           |

---

## Mimari — Memento Deseni Rolleri

```
MementoEditorDemo/
├── Models/
│   ├── EditorMemento.cs   ← Memento    — durumu kapsüller, dışarıya kapalı
│   ├── TextEditor.cs      ← Originator — durumu olan asıl nesne
│   └── EditorHistory.cs   ← Caretaker  — Memento'ları saklar, içine bakmaz
├── Services/
│   ├── SaveMode.cs        ← Enum       — Snapshot | Delta
│   └── EditorService.cs   ← Business Logic — UI ile Models arası köprü
├── Pages/
│   └── Editor.razor       ← UI
└── Shared/
    ├── MainLayout.razor
    └── NavMenu.razor
```

### Kritik Tasarım Kararı

`EditorMemento` sınıfının constructor'ı `internal` tanımlanmıştır. Bu sayede **Caretaker yeni bir Memento üretemez**; yetki yalnızca aynı assembly içindeki Originator'a aittir — kapsülleme (encapsulation) ilkesi kusursuz korunur.

---

## Kurulum ve Çalıştırma

### Gereksinimler

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio Code + C# Dev Kit eklentisi

### Adımlar

```bash
# 1. Repoyu klonla
git clone https://github.com/barisgevher/MementoEditorDemo.git
cd MementoEditorDemo

# 2. Çalıştır (hot-reload ile)
dotnet watch run

# 3. Tarayıcıda aç
# http://localhost:5000
```

---

## Snapshot vs Delta — Fark Ne?

|                  | Snapshot        | Delta                      |
| ---------------- | --------------- | -------------------------- |
| **Kaydedilen**   | İçeriğin tamamı | Yalnızca değişen kısım     |
| **Bellek**       | Yüksek          | Düşük                      |
| **Geri yükleme** | Anlık           | Hesaplama gerektirir       |
| **Kullanım**     | Oyun save/load  | Git commit, veritabanı WAL |

Uygulamada her iki mod da aynı undo/redo güvenilirliğini sunar; mod yalnızca **gösterilen boyut hesabını** etkiler.

---

## Referanslar

- Gamma et al., _Design Patterns: Elements of Reusable Object-Oriented Software_, Addison-Wesley, 1994
- [Refactoring.Guru — Memento Pattern](https://refactoring.guru/design-patterns/memento)
- [Microsoft Docs — Blazor WebAssembly](https://learn.microsoft.com/aspnet/core/blazor/)

---
