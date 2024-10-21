namespace E_Retalling_Portal.Util
{
    public static class VectorUtils
    {
        public static double CosineSimilarity(float[]? vectorA, float[]? vectorB)
        {
            if(vectorA == null || vectorB == null) return 0;

            if (vectorA.Length != vectorB.Length)
            {
                throw new ArgumentException("length not equal");
            }

            double dotProduct = 0.0;
            double magnitudeA = 0.0;
            double magnitudeB = 0.0;

            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                magnitudeA += vectorA[i] * vectorA[i];
                magnitudeB += vectorB[i] * vectorB[i];
            }

            double magnitude = Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB);

            if (magnitude == 0)
            {
                return 0;
            }
            return dotProduct / magnitude;
        }
    }
}
