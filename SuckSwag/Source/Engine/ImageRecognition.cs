namespace SuckSwag.Source.GameState
{
    using AForge.Imaging;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public class ImageRecognition
    {
        private static ExhaustiveTemplateMatching recognition = new ExhaustiveTemplateMatching();

        /// <summary>
        /// Finds the best match for the given candidate image against the provided templates.
        /// </summary>
        /// <param name="candidates">The image being compared.</param>
        /// <param name="templates">The templates against which to compare the image.</param>
        /// <returns></returns>
        public static Bitmap BestMatch(IEnumerable<Bitmap> candidates, params Bitmap[] templates)
        {
            const float similarityThreshold = 0.25f;

            if (candidates == null || candidates.Count() <= 0)
            {
                return null;
            }

            Bitmap bestMatch = candidates
              // Get the similarity to all template images
              .Select(candidate =>
                  new
                  {
                      bitmap = candidate,
                      matchings = templates.Select(template => recognition.ProcessImage(candidate, template)),
                  })
              .Select(candidate =>
                  new
                  {
                      bitmap = candidate.bitmap,
                      similarity = candidate.matchings.Select(match => match.Count() > 0 ? match[0].Similarity : 0.0f).Max(),
                  })

               // Threshold the similarity
               .Where(board => board.similarity > similarityThreshold)

               // Pick the best
               .OrderByDescending(template => template.similarity)
               .FirstOrDefault()?.bitmap;

            return bestMatch;
        }
    }
    //// End class
}
//// End namespace