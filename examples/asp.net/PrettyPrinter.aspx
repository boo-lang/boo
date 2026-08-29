<%@Page Inherits="Boo.Examples.Web.PrettyPrinterPage" ValidateRequest="False" %>
<html>
<body>
<style type="text/css">
.code
{
	text-align: left;
	font-family: lucida console;
	font-size: 11pt;
	background-color: #DDDDDD;
	width: 70%;
	padding: 15px;
}
.operator { font-weight: bold; }
.keyword { font-weight: bold; color: blue; }
.string { color: white; }
.integer { color: blue; }
</style>
<form runat='server'>
<center>
Type in some boo code<br/>
<asp:TextBox id='_src' runat="server" TextMode="Multiline" Columns="80" Rows="20" />
<br />
<asp:Button runat="server" Text="print it!" />
<br />
<div id="_pretty" runat="server" class="code">
</div>
</center>

</form>
</body>
</html>
