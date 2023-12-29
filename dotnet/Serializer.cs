public static class Serializer {
  public static object? Read(Stream stream, IEnumerable<IReader> readers) {
    var reader = new AggregateReader(readers);
    return reader.Read(reader, stream);
  }
  public static bool Write(Stream stream, object? value, IEnumerable<IWriter> writers) {
    var writer = new AggregateWriter(writers);
    return writer.Write(writer, stream, value);
  }
  public interface IReader { public object? Read(IReader serializer, Stream stream); }
  public interface IWriter { public bool Write(IWriter serializer, Stream stream, object? value); }
  private class AggregateReader : IReader {
    private readonly IEnumerable<IReader> readers;
    public AggregateReader(IEnumerable<IReader> readers) => this.readers = readers;
    public object? Read(IReader serializer, Stream stream) {
      var position = stream.Position;
      foreach (var reader in readers) {
        var value = reader.Read(serializer, stream);
        if (stream.Position > position) return value;
      }
      throw new Exception("Unsupported Format");
    }
  }
  private class AggregateWriter : IWriter {
    private readonly IEnumerable<IWriter> writers;
    public AggregateWriter(IEnumerable<IWriter> writers) => this.writers = writers;
    public bool Write(IWriter serializer, Stream stream, object? value) {
      foreach (var writer in writers) {
        if (writer.Write(serializer, stream, value)) return true;
      }
      throw new Exception("Unsupported Value");
    }
  }
}