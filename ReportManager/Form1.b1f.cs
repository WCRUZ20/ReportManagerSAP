using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Xml;

namespace ReportManager
{
    [FormAttribute("ReportManager.Form1", "Form1.b1f")]
    class Form1 : UserFormBase
    {
        public Form1()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.StaticText2 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_7").Specific));
            this.StaticText3 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_8").Specific));
            this.Button1 = ((SAPbouiCOM.Button)(this.GetItem("btn_exe").Specific));
            this.Button1.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button1_ClickBefore);
            this.EditText0 = ((SAPbouiCOM.EditText)(this.GetItem("edt_dpt").Specific));
            this.EditText1 = ((SAPbouiCOM.EditText)(this.GetItem("edt_rpt").Specific));
            this.Folder0 = ((SAPbouiCOM.Folder)(this.GetItem("UI001").Specific));
            this.Folder1 = ((SAPbouiCOM.Folder)(this.GetItem("UI002").Specific));
            this.EditText2 = ((SAPbouiCOM.EditText)(this.GetItem("edt_sn").Specific));
            this.LinkedButton0 = ((SAPbouiCOM.LinkedButton)(this.GetItem("lkb_sn").Specific));
            this.EditText3 = ((SAPbouiCOM.EditText)(this.GetItem("edt_snds").Specific));
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_10").Specific));
            this.StaticText1 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_11").Specific));
            this.EditText4 = ((SAPbouiCOM.EditText)(this.GetItem("edt_fini").Specific));
            this.EditText5 = ((SAPbouiCOM.EditText)(this.GetItem("edt_ffin").Specific));
            this.StaticText4 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_14").Specific));
            this.StaticText5 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_15").Specific));
            this.OnCustomInitialize();
            this.inicomponentes();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
        }

        private void OnCustomInitialize()
        {

        }
        private SAPbouiCOM.StaticText StaticText2;
        private SAPbouiCOM.StaticText StaticText3;
        private SAPbouiCOM.Button Button1;

        private void Button1_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            //throw new System.NotImplementedException();
            //Application.SBO_Application.SetStatusBarMessage("Boton ejecutar presionado", SAPbouiCOM.BoMessageTime.bmt_Short, false);
            Application.SBO_Application.StatusBar.SetText("Boton ejecutar presionado", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
        }

        public void inicomponentes()
        {
            this.EditText4.Value = DateTime.Now.ToString("yyyyMMdd");
            this.EditText5.Value = DateTime.Now.ToString("yyyyMMdd");
        }


        private SAPbouiCOM.EditText EditText0;
        private SAPbouiCOM.EditText EditText1;
        private SAPbouiCOM.Folder Folder0;
        private SAPbouiCOM.Folder Folder1;
        private SAPbouiCOM.EditText EditText2;
        private SAPbouiCOM.LinkedButton LinkedButton0;
        private SAPbouiCOM.EditText EditText3;
        private SAPbouiCOM.StaticText StaticText0;
        private SAPbouiCOM.StaticText StaticText1;
        private SAPbouiCOM.EditText EditText4;
        private SAPbouiCOM.EditText EditText5;
        private SAPbouiCOM.StaticText StaticText4;
        private SAPbouiCOM.StaticText StaticText5;
    }
}