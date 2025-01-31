using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

public class Utils {
  public static string ToCamelCase(string input) {
    if (string.IsNullOrEmpty(input)) return input;

    string[] words = input.Split(new[] { ' ', '-', '_', '.' }, StringSplitOptions.RemoveEmptyEntries);

    // The givne input is probably already camel case
    if (words.Length == 1) return input;

    StringBuilder camelCaseString = new StringBuilder();
    TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;

    for (int i = 0; i < words.Length; i++) {
      string word = words[i].ToLower();
      if (i == 0) {
        camelCaseString.Append(word);
      } else {
        camelCaseString.Append(textInfo.ToTitleCase(word));
      }
    }
    return camelCaseString.ToString();
  }
}