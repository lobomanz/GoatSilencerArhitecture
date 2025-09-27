using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace GoatSilencerArchitecture.Services.Validation
{
    public class RichTextValidator : IRichTextValidator
    {
        public List<string> Validate(string? html)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(html)) return errors;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Heading hierarchy
            var headings = doc.DocumentNode.Descendants()
                .Where(n => n.Name.Length == 2 && n.Name.StartsWith("h") && int.TryParse(n.Name.Substring(1), out _))
                .ToList();

            var foundLevels = new HashSet<int>();

            foreach (var heading in headings)
            {
                int level = int.Parse(heading.Name.Substring(1));

                if (string.IsNullOrWhiteSpace(heading.InnerText))
                    errors.Add($"Heading <{heading.Name}> is empty.");

                for (int i = 1; i < level; i++)
                {
                    if (!foundLevels.Contains(i))
                    {
                        errors.Add($"Heading <h{level}> is invalid. You must define <h{i}> before using it.");
                        break;
                    }
                }

                foundLevels.Add(level);
            }

            // Links with same href must match text
            var links = doc.DocumentNode.Descendants("a")
                .Where(n => n.Attributes["href"] != null);

            var linkGroups = links.GroupBy(l => l.Attributes["href"].Value).Where(g => g.Count() > 1);

            foreach (var group in linkGroups)
            {
                var texts = group.Select(l => l.InnerText.Trim()).Distinct().ToList();
                if (texts.Count > 1)
                    errors.Add($"Links pointing to '{group.Key}' must have the same text, but found: {string.Join(", ", texts)}");
            }

            // Images need alt
            var images = doc.DocumentNode.Descendants("img");
            foreach (var img in images)
            {
                var alt = img.GetAttributeValue("alt", "");
                if (string.IsNullOrWhiteSpace(alt))
                    errors.Add("All images must have a non-empty alt attribute.");
            }

            // Empty links
            foreach (var link in links)
            {
                if (string.IsNullOrWhiteSpace(link.InnerText))
                    errors.Add($"Link with href '{link.Attributes["href"].Value}' has no visible text.");
            }

            return errors;
        }
    }
}
