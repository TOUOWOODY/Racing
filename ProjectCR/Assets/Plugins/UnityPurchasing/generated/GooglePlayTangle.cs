#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("kBlY/SnToPhtFqhaeKvoOh3u6fnWK1/XBfGz07WaO1Dy0WT29dR6QbQ3OTYGtDc8NLQ3NzaKY09/S33tP6hHXYhq/GJksf/1HTnKuqIM3UkyvFlnixs5OdY4nIy90TaShE+q5MCZJMTDGtyl6B+xbFgMQaHcFXUMlgAiCQJcAW5QcJBvnO+oxk/Pfcs3B88XUDCUfQEKIqOgw4TRujL2PjONMpj9+RqbV88aiTQBlpZlxZoWC9rcecgAj1C1cv3J9aepMBSNiN1eUTbW7rt9H/1T+mUehvWwu2CRMwY84BzkrXAE8GoOL2cSR4+7JOAbqJhtFkbFMhvWIChfnyD/1oGmpkgGtDcUBjswPxywfrDBOzc3NzM2NQaESgo8NJK+LTQ1NzY3");
        private static int[] order = new int[] { 3,7,7,12,5,9,10,11,13,10,13,11,13,13,14 };
        private static int key = 54;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
