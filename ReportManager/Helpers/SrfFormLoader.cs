using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ReportManager.Helpers
{
    internal static class SrfFormLoader
    {
        /// <summary>
        /// Carga un SRF y devuelve el formulario por su UID (leído del SRF).
        /// Si ya existe, retorna el existente.
        /// </summary>
        public static SAPbouiCOM.IForm Load(string srfFileNameOrFullPath)
        {
            // 1) Resolver ruta real
            var srfFullPath = ResolveSrfPath(srfFileNameOrFullPath);

            // 2) Leer XML y obtener UID del form desde el SRF
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(srfFullPath);

            string formUid = TryGetFormUid(xmlDoc);
            if (string.IsNullOrWhiteSpace(formUid))
                throw new Exception($"No se pudo leer el UID del formulario desde el SRF: {srfFullPath}");

            // 3) Si ya está abierto, devolverlo (equivalente a RecorreFormulario)
            var existing = TryGetFormByUid(formUid);
            if (existing != null)
            {
                existing.Select();
                existing.Visible = true;
                return existing;
            }

            // 4) Cargar por BatchActions (como VB)
            try
            {
                Application.SBO_Application.LoadBatchActions(xmlDoc.InnerXml);

                string batchResult = Application.SBO_Application.GetLastBatchResults() ?? string.Empty;
                if (LooksLikeBatchError(batchResult))
                {
                    // Si por alguna razón lo creó y dio error, intentar cerrar
                    TryCloseForm(formUid);
                    throw new Exception("Error cargando SRF (BatchResults): " + batchResult);
                }

                // 5) Obtener el form por UID (NO ActiveForm)
                var form = Application.SBO_Application.Forms.Item(formUid);
                form.Visible = true;
                return form;
            }
            catch
            {
                // Si algo explotó en medio, intentar cerrar si se creó
                TryCloseForm(formUid);
                throw;
            }
        }

        /// <summary>
        /// Helper opcional para trabajar como en tu VB: Freeze True y manejar finally.
        /// </summary>
        public static SAPbouiCOM.IForm LoadAndFreeze(string srfFileNameOrFullPath)
        {
            var form = Load(srfFileNameOrFullPath);
            try
            {
                form.Freeze(true);
                return form;
            }
            catch
            {
                try { form.Freeze(false); } catch { }
                throw;
            }
        }

        private static string ResolveSrfPath(string fileNameOrFullPath)
        {
            if (string.IsNullOrWhiteSpace(fileNameOrFullPath))
                throw new ArgumentException("Ruta SRF inválida.", nameof(fileNameOrFullPath));

            // Si ya viene full path
            if (Path.IsPathRooted(fileNameOrFullPath) && File.Exists(fileNameOrFullPath))
                return fileNameOrFullPath;

            // Base dir del add-on (normalmente bin\x64\Debug o bin\Release)
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // Candidatos típicos (ajusta/añade si deseas)
            var candidates = new[]
            {
                Path.Combine(baseDir, fileNameOrFullPath),
                Path.Combine(baseDir, "Forms", fileNameOrFullPath),
                Path.Combine(baseDir, "..", "..", "Forms", fileNameOrFullPath),
                Path.Combine(baseDir, "..", "..", "..", "Forms", fileNameOrFullPath),
            }
            .Select(p => Path.GetFullPath(p))
            .Distinct()
            .ToList();

            foreach (var c in candidates)
                if (File.Exists(c)) return c;

            throw new FileNotFoundException(
                "No se encontró el SRF. Probé:\n - " + string.Join("\n - ", candidates),
                fileNameOrFullPath
            );
        }

        private static string TryGetFormUid(XmlDocument doc)
        {
            // SRF típico: <form uid="frmAcercaDe" ...>
            // Puede venir bajo: <Application><forms><action type="add"><form .../></action></forms></Application>
            var node = doc.SelectSingleNode("//form[@uid]") ?? doc.SelectSingleNode("//Form[@uid]");
            return node?.Attributes?["uid"]?.Value?.Trim();
        }

        private static SAPbouiCOM.IForm TryGetFormByUid(string uid)
        {
            try
            {
                return Application.SBO_Application.Forms.Item(uid);
            }
            catch
            {
                return null;
            }
        }

        private static void TryCloseForm(string uid)
        {
            try
            {
                var f = Application.SBO_Application.Forms.Item(uid);
                f.Close();
            }
            catch { }
        }

        private static bool LooksLikeBatchError(string batchResult)
        {
            // Batch results a veces trae XML, a veces texto.
            // Si trae "Error" o status != 0, lo tomamos como error.
            if (string.IsNullOrWhiteSpace(batchResult)) return false;

            if (batchResult.IndexOf("error", StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            // Intento simple: si es XML con <status>0</status>, ok; caso contrario, error.
            try
            {
                var x = new XmlDocument();
                x.LoadXml(batchResult);

                var statusNode = x.SelectSingleNode("//status") ?? x.SelectSingleNode("//Status");
                if (statusNode != null && int.TryParse(statusNode.InnerText.Trim(), out int st))
                    return st != 0;

                var errNode = x.SelectSingleNode("//error") ?? x.SelectSingleNode("//Error");
                return errNode != null;
            }
            catch
            {
                // si no es XML y no contiene error, asumimos OK
                return false;
            }
        }
    }
}
