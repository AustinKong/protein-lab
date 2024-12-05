using System.Collections.Generic;

public class CombinationRules
{
  public static Dictionary<string, string> combinations = new Dictionary<string, string>{
    { "Deionized water + Glass bottle", "Glass of deionized water" },
    { "Glass of deionized water + Buffer salts", "Unmixed crude buffer solution" },
    { "Unmixed crude buffer solution + Magnetic stirring plate", "Crude buffer solution" },
    // Two paths
    { "Crude buffer solution + Acid", "Acidic buffer solution" },
    { "Crude buffer solution + Base", "Basic buffer solution" },
    { "Acidic buffer solution + Base", "Buffer solution" },
    { "Basic buffer solution + Acid", "Buffer solution" },
  };

  public static string GetCombinationResult(string item1, string item2) {
    if (combinations.TryGetValue(item1.Trim() + " + " + item2.Trim(), out string result)) {
      return result;
    } else if (combinations.TryGetValue(item2.Trim() + " + " + item1.Trim(), out result)) {
      return result;
    }
    return null;
  }
}
