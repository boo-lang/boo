<%@Page Inherits="Boo.Examples.Web.ScriptRunnerPage" ValidateRequest="False" %>
<html>
<body>
<form runat='server'>
<center>
Type in some boo code and press <b>run</b><br/>
<asp:TextBox id='_code' runat="server" TextMode="Multiline" Columns="80" Rows="20" />
<br />
<asp:Button id="_run" runat="server" Text="run" OnClick="_run_Click" />
<br />
<div id="_console" runat="server" >
</div>
</center>

</form>
</body>
</html>
