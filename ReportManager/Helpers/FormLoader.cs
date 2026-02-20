using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportManager.Helpers
{
    internal static class FormLoader
    {
        public static void LoadSrf(string srfPath)
        {
            string xml = File.ReadAllText(srfPath);

            Application.SBO_Application.LoadBatchActions(xml);

            // Si hubo error, SAP lo deja en el log del batch
            string result = Application.SBO_Application.GetLastBatchResults();
            if (!string.IsNullOrWhiteSpace(result) && result.IndexOf("Error", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new Exception("Error cargando SRF: " + result);
            }
        }
    }
}
