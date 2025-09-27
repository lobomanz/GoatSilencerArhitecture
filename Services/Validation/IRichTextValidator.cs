using System.Collections.Generic;

namespace GoatSilencerArchitecture.Services.Validation
{
    public interface IRichTextValidator
    {
        /// <summary>
        /// Validates the given HTML string against accessibility rules.
        /// </summary>
        /// <param name="html">The HTML content to validate.</param>
        /// <returns>List of validation error messages. Empty if valid.</returns>
        List<string> Validate(string? html);
    }
}
