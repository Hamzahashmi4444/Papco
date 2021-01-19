<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="ReportViewerWebForm.aspx.cs" Inherits="ReportViewerForMvc.ReportViewerWebForm" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

        <script runat="server">

protected void Page_Init(Object sender, EventArgs e)
{

    DisableUnwantedExportFormat(ReportViewer1, "PDF");
    DisableUnwantedExportFormat(ReportViewer1, "Word");
    DisableUnwantedExportFormat(ReportViewer1, "WORDOPENXML");
    

    
}

public void DisableUnwantedExportFormat(ReportViewer ReportViewerID, string strFormatName)
{
    System.Reflection.FieldInfo info;

    foreach (RenderingExtension extension in ReportViewerID.LocalReport.ListRenderingExtensions())
    {
        if (extension.Name == strFormatName)
        {
            info = extension.GetType().GetField("m_isVisible", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            info.SetValue(extension, false);
        }
    }
}


</script>
</head>
<body style="margin: 0px; padding: 0px;">
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
                <Scripts>
                    <asp:ScriptReference Assembly="ReportViewerForMvc" Name="ReportViewerForMvc.Scripts.PostMessage.js" />
                </Scripts>
            </asp:ScriptManager>
            <rsweb:ReportViewer ID="ReportViewer1" runat="server"></rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
