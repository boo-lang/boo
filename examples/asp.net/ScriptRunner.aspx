<%@Page Inherits="Boo.Examples.Web.ScriptRunnerPage" ValidateRequest="False" %>
<html>
<body>
<form runat='server'>
<center>
Type in some boo code and press <b>run</b><br/>
Try <span style='font-family: lucida console; color: white; background-color: #ddccdd'>print(Request.Url)</span>, for instance<br />
<asp:TextBox id='_code' runat="server" TextMode="Multiline" Columns="80" Rows="20" style="font-family: lucida console;" />
<br />
<asp:Button id="_run" runat="server" Text="run" OnClick="_run_Click" />
<br />
<div style="padding: 10px; background-color: silver; width: 90%">
<div id="_console" runat="server" style="font-family: lucida console; text-align: left" >
</div>
</div>
</center>

</form>
</body>
</html>
