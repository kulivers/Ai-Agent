using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FormulaParser;

public static class FormulaXmlParser
{
    public static List<FormulaDescription> Parse(string xmlFilePath)
    {
        var doc = XDocument.Load(xmlFilePath);
        return ParseDocument(doc);
    }

    public static List<FormulaDescription> ParseFromString(string xmlContent)
    {
        var doc = XDocument.Parse(xmlContent);
        return ParseDocument(doc);
    }

    private static List<FormulaDescription> ParseDocument(XDocument doc)
    {
        var formulas = new List<FormulaDescription>();
        var tables = doc.Descendants("table");

        foreach (var table in tables)
        {
            var formula = ParseTable(table);
            if (formula != null)
            {
                formulas.Add(formula);
            }
        }

        return formulas;
    }

    private static FormulaDescription? ParseTable(XElement table)
    {
        var formula = new FormulaDescription();

        var header = table.Descendants("h3").FirstOrDefault();
        if (header == null)
            return null;

        var nameCode = header.Descendants("code").FirstOrDefault();
        if (nameCode == null)
            return null;

        formula.Name = nameCode.Value.Trim().TrimEnd('(', ')');

        var rows = table.Descendants("tr").Skip(1);

        foreach (var row in rows)
        {
            var cells = row.Elements("td").ToList();
            if (cells.Count != 2)
                continue;

            var label = GetTextContent(cells[0]).Trim();
            var content = cells[1];

            switch (label)
            {
                case "Описание":
                    formula.Description = GetTextContent(content).Trim();
                    break;

                case "Синтаксис":
                    formula.Syntax = ExtractSyntax(content);
                    break;

                case "Аргументы":
                    formula.Arguments = GetTextContent(content).Trim();
                    break;

                case "Результат":
                    formula.Result = GetTextContent(content).Trim();
                    break;

                case "Пример":
                    formula.Example = ExtractExample(content);
                    formula.ExampleResult = ExtractExampleResult(content);
                    break;
            }
        }

        return formula;
    }

    private static string GetTextContent(XElement element)
    {
        var sb = new StringBuilder();

        foreach (var node in element.DescendantNodes())
        {
            if (node is XText textNode)
            {
                sb.Append(textNode.Value);
            }
        }

        var text = sb.ToString();
        text = Regex.Replace(text, @"\s+", " ");
        return text.Trim();
    }

    private static string ExtractSyntax(XElement element)
    {
        var codeBlock = element.Descendants("pre").FirstOrDefault();
        if (codeBlock == null)
            return string.Empty;

        var sb = new StringBuilder();

        foreach (var span in codeBlock.Descendants("span"))
        {
            sb.Append(span.Value);
        }

        var syntax = sb.ToString().Trim();
        syntax = Regex.Replace(syntax, @"\s+", " ");

        return syntax;
    }

    private static string ExtractExample(XElement element)
    {
        var codeBlock = element.Descendants("pre").FirstOrDefault();
        if (codeBlock == null)
            return string.Empty;

        var sb = new StringBuilder();

        foreach (var span in codeBlock.Descendants("span"))
        {
            var text = span.Value;
            text = System.Net.WebUtility.HtmlDecode(text);
            sb.Append(text);
        }

        var example = sb.ToString().Trim();
        example = Regex.Replace(example, @"\s+", " ");

        return example;
    }

    private static string? ExtractExampleResult(XElement element)
    {
        var resultParagraph = element.Descendants("p")
            .FirstOrDefault(p => GetTextContent(p).StartsWith("Результат:", StringComparison.OrdinalIgnoreCase));

        if (resultParagraph == null)
            return null;

        var text = GetTextContent(resultParagraph);

        var match = Regex.Match(text, @"Результат:\s*(.+)", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            return match.Groups[1].Value.Trim();
        }

        return null;
    }
}
