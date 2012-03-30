<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MVCAzureStore.Models.IndexViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
Azure Store Products
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Products</h1>
    <label for="items">Select a product from the list:</label>
    <h3>Instance ID: <%: Model.InstanceId %> - Object ID: <%: Model.ObjectId %></h3>
    <% using (Html.BeginForm("Add", "Home")) { %>
        <select name="selectedItem" class="product-list" id="items" size="4">
        <% foreach (string product in Model.Products) { %>
            <option value="<%: product%>"><%: product%></option>
        <% } %>
        </select>
        <a href="javascript:document.forms[0].submit();">Add item to cart</a>
    <% } %>
    <fieldset>
        <legend>Cache settings for product data</legend>Enable Cache:
        <%if (Model.IsCacheEnabled)
          { %>
            Yes | <%=Html.ActionLink("No", "EnableCache", new { enabled = false }).ToString()%>
        <%}
          else
          { %>
            <%=Html.ActionLink("Yes", "EnableCache", new { enabled = true }).ToString()%> | No
        <%} %>     
        <br />
        <%if (Model.IsCacheEnabled)
        { %>
        Use Local Cache:
        <%if (Model.IsLocalCacheEnabled)
              { %>
        Yes |
        <%=Html.ActionLink("No", "EnableLocalCache", new { enabled = false }).ToString()%>
        <%}
              else
              { %>
        <%=Html.ActionLink("Yes", "EnableLocalCache", new { enabled = true }).ToString()%>
        | No
        <%} %>
        <%} %>   
        <div id="elapsedTime">Elapsed time: <%:Model.ElapsedTime.ToString()%> milliseconds.</div>
    </fieldset>    
    
</asp:Content>
