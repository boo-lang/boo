<%@Page Inherits="Boo.Examples.Web.PrettyPrinterPage" ValidateRequest="False" %>
<html>
<body>
<style type="text/css">
.code { text-align: left; font-family: lucida console; font-size: 11pt; }
.operator { font-weight: bold; }
.keyword { font-weight: bold; color: blue; }
.string { color: orange; }
</style>
<form runat='server'>
<center>
<asp:TextBox id='_srcCode' runat="server" TextMode="Multiline" Columns="80" Rows="20" />
<br />
<asp:Button runat="server" Text="print it!" />
<br />
<div id="_printedCode" runat="server" class="code" style="width: 70%">
</div>
</center>

</form>
</body>
</html>
