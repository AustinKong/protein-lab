public interface IConsumer {
  public bool CanConsume(string otherItemName);
  public void Consume(string otherItemName);
}