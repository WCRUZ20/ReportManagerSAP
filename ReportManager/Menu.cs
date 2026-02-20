using ReportManager.Helpers;
using SAPbouiCOM.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportManager
{
    class Menu
    {
        public void AddMenuItems()
        {
            SAPbouiCOM.Menus oMenus = null;
            SAPbouiCOM.MenuItem oMenuItem = null;

            oMenus = Application.SBO_Application.Menus;

            SAPbouiCOM.MenuCreationParams oCreationPackage = null;
            oCreationPackage = ((SAPbouiCOM.MenuCreationParams)(Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams)));
            oMenuItem = Application.SBO_Application.Menus.Item("43520"); // moudles'

            oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_POPUP;
            oCreationPackage.UniqueID = "ReportManager";
            oCreationPackage.String = "Gestor de reportes";
            oCreationPackage.Enabled = true;
            oCreationPackage.Position = -1;

            oMenus = oMenuItem.SubMenus;

            try
            {
                //  If the manu already exists this code will fail
                oMenus.AddEx(oCreationPackage);
            }
            catch (Exception e)
            {

            }

            try
            {
                // Get the menu collection of the newly added pop-up item
                oMenuItem = Application.SBO_Application.Menus.Item("ReportManager");
                oMenus = oMenuItem.SubMenus;

                // Create s sub menu
                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "ReportManager.Form1";
                oCreationPackage.String = "Generar reporte";
                oMenus.AddEx(oCreationPackage);

                oCreationPackage.Type = SAPbouiCOM.BoMenuType.mt_STRING;
                oCreationPackage.UniqueID = "ReportManager.Principal";
                oCreationPackage.String = "Generar reporte (SRF)";
                oMenus.AddEx(oCreationPackage);
            }
            catch (Exception er)
            { //  Menu already exists
                //Application.SBO_Application.SetStatusBarMessage("Menu Already Exists", SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }

        public void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                if (pVal.BeforeAction && pVal.MenuUID == "ReportManager.Form1")
                {
                    Form1 activeForm = new Form1();
                    activeForm.Show();
                }

                if (pVal.MenuUID == "ReportManager.Principal")
                {
                    //string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                    //string srfPath = System.IO.Path.Combine(baseDir, "Forms", "Principal.srf");

                    //var form = ReportManager.Helpers.SrfFormLoader.LoadFromFile(srfPath);

                    var oForm = SrfFormLoader.Load(@"Principal.srf"); // o @"C:\...\Forms\Principal.srf"
                    oForm.Freeze(true);
                    try
                    {
                        // set datasources, binds, etc...
                    }
                    finally
                    {
                        oForm.Freeze(false);
                    }

                    //form.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;

                    return;
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(ex.ToString(), 1, "Ok", "", "");
            }
        }

    }
}
