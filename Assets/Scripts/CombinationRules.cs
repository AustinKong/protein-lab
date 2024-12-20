using System.Collections.Generic;

public class CombinationRules
{
  public static Dictionary<string, string> combinations = new Dictionary<string, string>{
    // Buffer solution preparation
    { "Buffer salt + Deionized water", "Unmixed crude buffer solution" },
    { "Deionized water + Buffer salt", "Unmixed crude buffer solution" },
    { "Unmixed crude buffer solution + Stirring plate", "Crude buffer solution" },
    // Two paths
    { "Hydrochloride acid + Crude buffer solution", "Acidic buffer solution" },
    { "Sodium hydroxide + Crude buffer solution", "Basic buffer solution" },
    { "Sodium hydroxide + Acidic buffer solution", "Buffer solution" },
    { "Hydrochloride acid + Basic buffer solution", "Buffer solution" },

    // Protein solution preparation
    { "Buffer solution + Centrifuge tube", "Buffer solution" },
    { "Protein + Buffer solution", "Untreated protein solution" },
    { "Buffer solution + Protein", "Untreated protein solution" },
    { "Untreated protein solution + Centrifuge", "Treated protein solution" },
    { "Treated protein solution + Syringe", "Filtered protein solution" },

    // Testing
    { "Sodium Hydroxide + Hydrogen Chloride", "Sodium Chloride" },
  };

  public static string GetCombinationResult(string consumer, string consumable) {
    if (combinations.TryGetValue($"{consumable.Trim()} + {consumer.Trim()}", out string result)) {
      return result;
    } else if (combinations.TryGetValue($"[{consumable.Trim()}] + {consumer.Trim()}", out result)) {
      return result;
    } else {
      return null;
    }
  }

/*
  public static string GetCombinationResult(string item1, string item2) {
    if (combinations.TryGetValue(item1.Trim() + " + " + item2.Trim(), out string result)) {
      return result;
    } else if (combinations.TryGetValue(item2.Trim() + " + " + item1.Trim(), out result)) {
      return result;
    }
    return null;
  }
  */
}
