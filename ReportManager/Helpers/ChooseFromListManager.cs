using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbouiCOM;
using SAPbouiCOM.Framework;

namespace ReportManager.Helpers
{
    public class ChooseFromListManager
    {
        private readonly Dictionary<string, BindingInfo> _bindings = new Dictionary<string, BindingInfo>(StringComparer.OrdinalIgnoreCase);

        private class BindingInfo
        {
            public string UdsId;
            public string AliasToSet;
        }

        /// <summary>
        /// Crea UDS si no existe, bindea el EditText, crea CFL si no existe (con condición) y lo asigna al EditText.
        /// Además registra el binding para que en ChooseFromListAfter se setee el valor.
        /// </summary>
        public void CrearChooseFromList(
            SAPbouiCOM.IForm oForm,
            string chooseId,
            string objectType,
            string campoAFiltrar,
            string condicion,
            string editTextItemId,
            string idDataSource,
            string chooseFromListAlias,
            bool multiSelect = false
        )
        {
            try
            {
                if (!ExisteUserDataSource(oForm, idDataSource))
                    oForm.DataSources.UserDataSources.Add(idDataSource, BoDataType.dt_SHORT_TEXT, 100);

                var oEdit = (EditText)oForm.Items.Item(editTextItemId).Specific;
                oEdit.DataBind.SetBound(true, "", idDataSource);

                if (!CFLExiste(oForm, chooseId))
                {
                    var cflParams = (ChooseFromListCreationParams)
                        SAPbouiCOM.Framework.Application.SBO_Application.CreateObject(BoCreatableObjectType.cot_ChooseFromListCreationParams);

                    cflParams.UniqueID = chooseId;
                    cflParams.ObjectType = objectType;     
                    cflParams.MultiSelection = multiSelect;

                    ChooseFromList cfl = oForm.ChooseFromLists.Add(cflParams);

                    if (!string.IsNullOrWhiteSpace(campoAFiltrar) && !string.IsNullOrWhiteSpace(condicion))
                    {
                        Conditions cons = cfl.GetConditions();
                        Condition con = cons.Add();
                        con.Alias = campoAFiltrar; 
                        con.Operation = BoConditionOperation.co_EQUAL;
                        con.CondVal = condicion;
                        cfl.SetConditions(cons);
                    }
                }

                oEdit.ChooseFromListUID = chooseId;
                oEdit.ChooseFromListAlias = chooseFromListAlias;

                _bindings[editTextItemId] = new BindingInfo
                {
                    UdsId = idDataSource,
                    AliasToSet = chooseFromListAlias
                };
            }
            catch (Exception ex)
            {
                SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText(
                    "Error al crear ChooseFromList: " + ex.Message,
                    BoMessageTime.bmt_Short,
                    BoStatusBarMessageType.smt_Error);

                throw;
            }
        }

        /// <summary>
        /// Handler robusto: toma el valor seleccionado (AliasToSet) y lo escribe en:
        /// 1) EditText.Value
        /// 2) UserDataSource.ValueEx (si existe mapping)
        /// </summary>
        public void HandleChooseFromListAfter(SAPbouiCOM.IForm oForm, string itemUid, SAPbouiCOM.IChooseFromListEvent cflEv)
        {
            try
            {
                if (!_bindings.ContainsKey(itemUid))
                    return;

                if (cflEv.SelectedObjects == null)
                    return;

                var info = _bindings[itemUid];
                var dt = cflEv.SelectedObjects;

                string value = TryGetValue(dt, info.AliasToSet, 0);
                if (string.IsNullOrWhiteSpace(value)) return;

                value = value.Trim();

                var edt = (SAPbouiCOM.EditText)oForm.Items.Item(itemUid).Specific;
                edt.Value = value;

                if (!string.IsNullOrWhiteSpace(info.UdsId) && ExisteUserDataSource(oForm, info.UdsId))
                    oForm.DataSources.UserDataSources.Item(info.UdsId).ValueEx = value;
            }
            catch (Exception ex)
            {
                SAPbouiCOM.Framework.Application.SBO_Application.StatusBar.SetText(
                    "CFL After error (manager): " + ex.Message,
                    SAPbouiCOM.BoMessageTime.bmt_Short,
                    SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
        }

        private static bool ExisteUserDataSource(SAPbouiCOM.IForm oForm, string udsId)
        {
            try { var _ = oForm.DataSources.UserDataSources.Item(udsId); return true; }
            catch { return false; }
        }

        private static bool CFLExiste(SAPbouiCOM.IForm oForm, string cflId)
        {
            try { var _ = oForm.ChooseFromLists.Item(cflId); return true; }
            catch { return false; }
        }

        private static string TryGetValue(DataTable dt, string colName, int row)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(colName) && dt.Columns.Item(colName) != null)
                    return dt.GetValue(colName, row)?.ToString();
            }
            catch { /* ignore */ }

            // fallback: primera columna
            try { return dt.GetValue(0, row)?.ToString(); }
            catch { return null; }
        }
    }
}
