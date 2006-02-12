<%@ Page Language="Boo" %>
<script runat="server">
		def Page_Load(Sender as Object, E as EventArgs) :
			HelloWorld.Text = "Hello World From Boo in ASP!"  
			//test comment
</script>
<html>

<head>
<title>ASP.NET Hello World</title>
</head>
<body bgcolor="#FFFFFF">

<!--<p><%= "Hello World!" %></p> -->
<p><asp:label id="HelloWorld" runat="server" /></p>

</body>
</html>
