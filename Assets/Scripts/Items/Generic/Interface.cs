public interface IConsumer {
  // string can be "Contents" or "Container"
  public string Consume(string consumable);
  public bool CanConsume(string consumable);
}