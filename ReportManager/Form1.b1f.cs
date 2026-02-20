using ReportManager.Helpers;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Xml;

namespace ReportManager
{
    [FormAttribute("ReportManager.Form1", "Form1.b1f")]
    class Form1 : UserFormBase
    {
        private ChooseFromListManager _cflManager;
        private static bool _itemEventHooked = false;

        public Form1()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_10").Specific));
            this.StaticText1 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_11").Specific));
            this.StaticText2 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_7").Specific));
            this.StaticText3 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_8").Specific));
            this.StaticText4 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_14").Specific));
            this.StaticText5 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_15").Specific));

            this.EditText0 = ((SAPbouiCOM.EditText)(this.GetItem("edt_dpt").Specific));
            this.EditText1 = ((SAPbouiCOM.EditText)(this.GetItem("edt_rpt").Specific));
            this.EditText2 = ((SAPbouiCOM.EditText)(this.GetItem("edt_sn").Specific));

            this.EditText3 = ((SAPbouiCOM.EditText)(this.GetItem("edt_snds").Specific));
            this.EditText4 = ((SAPbouiCOM.EditText)(this.GetItem("edt_fini").Specific));
            this.EditText5 = ((SAPbouiCOM.EditText)(this.GetItem("edt_ffin").Specific));

            this.Button1 = ((SAPbouiCOM.Button)(this.GetItem("btn_exe").Specific));
            this.Button1.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button1_ClickBefore);
            
            this.Folder0 = ((SAPbouiCOM.Folder)(this.GetItem("UI001").Specific));
            this.Folder1 = ((SAPbouiCOM.Folder)(this.GetItem("UI002").Specific));

            this.LinkedButton0 = ((SAPbouiCOM.LinkedButton)(this.GetItem("lkb_sn").Specific));

            this.inicomponentes();
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            if (!_itemEventHooked)
            {
                _itemEventHooked = true;
                Application.SBO_Application.ItemEvent += SBO_Application_ItemEvent;
            }
        }

        private void OnCustomInitialize()
        {

        }

        private void Button1_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            //throw new System.NotImplementedException();
            //Application.SBO_Application.SetStatusBarMessage("Boton ejecutar presionado", SAPbouiCOM.BoMessageTime.bmt_Short, false);
            Application.SBO_Application.StatusBar.SetText("Boton ejecutar presionado..", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
        }

        public void inicomponentes()
        {
            this.EditText4.Value = DateTime.Now.ToString("yyyyMMdd");
            this.EditText5.Value = DateTime.Now.ToString("yyyyMMdd");

            _cflManager = new ChooseFromListManager();

            // Registrar CFL para OCRD en edt_sn

            _cflManager.CrearChooseFromList(
                oForm: this.UIAPIRawForm,
                chooseId: "CFL_SN",
                objectType: "2",               // OCRD (Business Partner)
                campoAFiltrar: "CardType",     // campo en OCRD
                condicion: "C",                // "C" cliente | "S" proveedor | "L" lead
                editTextItemId: "edt_sn",
                idDataSource: "UDS_SN",
                chooseFromListAlias: "CardCode",
                multiSelect: false
            );
        }

        //private void EditText2_ChooseFromListAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        //{
        //    // Esto fuerza que SIEMPRE se escriba el CardCode al edittext y al UDS
        //    _cflManager.HandleChooseFromListAfter(this.UIAPIRawForm, pVal);
        //}

        private void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                SAPbouiCOM.IForm form;
                try
                {
                    form = Application.SBO_Application.Forms.Item(FormUID);
                }
                catch
                {
                    return;
                }

                if (!string.Equals(form.TypeEx, "ReportManager.Form1", StringComparison.OrdinalIgnoreCase))
                    return;

                if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST && !pVal.BeforeAction)
                {
                    var cflEv = (SAPbouiCOM.IChooseFromListEvent)pVal;

                    if (pVal.ItemUID == "edt_sn")
                    {
                        if (_cflManager == null) _cflManager = new ReportManager.Helpers.ChooseFromListManager();

                        _cflManager.HandleChooseFromListAfter(form, pVal.ItemUID, cflEv);
                    }
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(
                    "CFL handler error: " + ex.Message,
                    SAPbouiCOM.BoMessageTime.bmt_Short,
                    SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
        }

        //labels
        private SAPbouiCOM.StaticText StaticText0;
        private SAPbouiCOM.StaticText StaticText1;
        private SAPbouiCOM.StaticText StaticText2;
        private SAPbouiCOM.StaticText StaticText3;
        private SAPbouiCOM.StaticText StaticText4;
        private SAPbouiCOM.StaticText StaticText5;

        //textboxes
        private SAPbouiCOM.EditText EditText0;
        private SAPbouiCOM.EditText EditText1;
        private SAPbouiCOM.EditText EditText2;
        private SAPbouiCOM.EditText EditText3;
        private SAPbouiCOM.EditText EditText4;
        private SAPbouiCOM.EditText EditText5;

        //buttons
        private SAPbouiCOM.Button Button1;
        
        //tabcontrols
        private SAPbouiCOM.Folder Folder0;
        private SAPbouiCOM.Folder Folder1;
        
        //linkedbuttons
        private SAPbouiCOM.LinkedButton LinkedButton0;
    }
}