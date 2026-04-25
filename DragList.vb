Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.Control
Imports System.Reflection
Imports System.Drawing
Imports System.IO
Imports HTC = System.Web.UI.HtmlControls
Imports Newtonsoft.Json

<DefaultProperty("Text"), ToolboxData("<{0}:DragList runat=server></{0}:DragList>")>
Public Class DragList
    Inherits WebControl
    'Implements ICallbackEventHandler

    'Implements IScriptControl

#Region "Variables"
    Private ulList As New HTC.HtmlGenericControl("ul")
    Private hfSelectedIndex As New HiddenField
    Private hfScrollTop As New HiddenField
    Private hfAutoPostBack As New HiddenField
    Private hfDoPostBack As New HiddenField
    Private hfIsFocused As New HiddenField
    Private hfAdjustScroll As New HiddenField

    'Private ulList As HTC.HtmlGenericControl
    'Private hfSelectedIndex As HiddenField
    'Private hfScrollTop As HiddenField
    'Private hfAutoPostBack As HiddenField
    'Private hfDoPostBack As HiddenField
    'Private hfIsFocused As HiddenField

    Private Loading As Boolean

    'Private sm As ScriptManager

#End Region

#Region "Enums"
    Public Enum HeaderAlign
        Left = 0
        Center = 1
        right = 2
    End Enum
#End Region

#Region "Classes"
    <Serializable> Public Class DragItem
        Public Text As String
        Public ID As String
        Public Tag As String
    End Class
    <Serializable>
    Public Class DragListItems
        Public Items As DragItem()
    End Class
    Public Class DragListEventArgs
        Inherits System.EventArgs

        Private mDragItem As DragItem
        Private mSelectedIndex As Integer
        Public Sub New(item As DragItem, index As Integer)
            mDragItem = item
            mSelectedIndex = index
        End Sub
        Public ReadOnly Property DragListItem As DragItem
            Get
                Return mDragItem
            End Get
        End Property
        Public ReadOnly Property SelectedIndex As Integer
            Get
                Return mSelectedIndex
            End Get
        End Property
    End Class
#End Region

#Region "EventDefinitions"
    Public Delegate Sub DragListEventHandler(sender As Object, e As DragListEventArgs)
    Public Event SelectedIndexChanged As DragListEventHandler
    Protected Overridable Sub OnSelectedIndexChanged(e As DragListEventArgs)
        RaiseEvent SelectedIndexChanged(Me, e)
    End Sub
#End Region

#Region "Properties"

    <Bindable(True),
     Category("Heading Appearance"),
     DefaultValue(""),
     Localizable(True)>
    Property Text() As String
        Get
            Dim s As String = CStr(ViewState("Text"))
            If s Is Nothing Then
                Return String.Empty
            Else
                Return s
            End If
        End Get

        Set(ByVal Value As String)
            ViewState("Text") = Value
        End Set
    End Property
    <Category("Behavior"), DefaultValue(True)>
    Property Draggable As Boolean
        Get
            If ViewState("Draggable") IsNot Nothing Then
                Return CBool(ViewState("Draggable"))
            Else
                ViewState("Draggable") = True
                Return True
            End If
        End Get
        Set(value As Boolean)
            Dim oldValue As Boolean = True
            If ViewState("Draggable") IsNot Nothing Then _
                oldValue = CBool(ViewState("Draggable"))
            If oldValue <> value Then
                ViewState("Draggable") = value
            End If

        End Set
    End Property

    <Category("Behavior"), DefaultValue(False)>
    Property DropOK As Boolean
        Get
            If ViewState("DropOK") IsNot Nothing Then
                Return CBool(ViewState("DropOK"))
            Else
                ViewState("DropOK") = False
                Return False
            End If
        End Get
        Set(value As Boolean)
            Dim oldValue As Boolean = False
            If ViewState("DropOK") IsNot Nothing Then _
                oldValue = CBool(ViewState("DropOK"))
            If oldValue <> value Then
                ViewState("DropOK") = value
            End If
        End Set
    End Property

    <Category("Heading Appearance"), DefaultValue("center")>
    Public Property HeadingAlignment As HeaderAlign
        Get
            If ViewState("HeadingAlignment") IsNot Nothing Then
                Return CType(ViewState("HeadingAlignment"), HeaderAlign)
            Else
                ViewState("HeadingAlignment") = HeaderAlign.Center
                Return HeaderAlign.Center
            End If

        End Get
        Set(value As HeaderAlign)
            ViewState("HeadingAlignment") = value
        End Set
    End Property

    <Category("Heading Appearance"), DefaultValue(GetType(Unit), "20px")>
    Public Property HeadingHeight As Unit
        Get
            If ViewState("HeadingHeight") IsNot Nothing Then
                Return CType(ViewState("HeadingHeight"), Unit)
            Else
                ViewState("HeadingHeight") = Unit.Parse("20px")
                Return Unit.Parse("20px")
            End If

        End Get
        Set(value As Unit)
            ViewState("HeadingHeight") = value
        End Set
    End Property

    <Category("Heading Appearance"), DefaultValue(GetType(Drawing.Color), "LightGray")>
    Property HeadingBackColor As Color
        Get
            If ViewState("HeadingBackColor") IsNot Nothing Then
                Return CType(ViewState("HeadingBackColor"), Color)
            Else
                ViewState("HeadingBackColor") = Color.LightGray
                Return Color.LightGray
            End If
        End Get
        Set(value As Color)
            ViewState("HeadingBackColor") = value
        End Set
    End Property

    <Category("Heading Appearance"), DefaultValue(GetType(Drawing.Color), "Black")>
    Property HeadingForeColor As Color
        Get
            If ViewState("HeadingForeColor") IsNot Nothing Then
                Return CType(ViewState("HeadingForeColor"), Color)
            Else
                ViewState("HeadingForeColor") = Color.Black
                Return Color.Black
            End If
        End Get
        Set(value As Color)
            ViewState("HeadingForeColor") = value
        End Set
    End Property

    <Category("Behavior"), DefaultValue(True)>
    Public Property HasHeader As Boolean
        Get
            If ViewState("HasHeader") IsNot Nothing Then
                Return CBool(ViewState("HasHeader"))
            Else
                ViewState("HasHeader") = True
                Return True
            End If
        End Get
        Set(value As Boolean)
            Dim oldValue As Boolean = True
            If ViewState("HasHeader") IsNot Nothing Then _
                oldValue = CBool(ViewState("HasHeader"))
            If oldValue <> value Then
                ViewState("HasHeader") = value
            End If
        End Set
    End Property
    <Category("Behavior"), DefaultValue(True)>
    Public Property DoPostBack As Boolean
        Get
            If ViewState("DoPostBack") IsNot Nothing Then
                Return CBool(ViewState("DoPostBack"))
            Else
                ViewState("DoPostBack") = True
                Return True
            End If
        End Get
        Set(value As Boolean)
            Dim oldValue As Boolean = True
            If ViewState("DoPostBack") IsNot Nothing Then _
                oldValue = CBool(ViewState("DoPostBack"))
            If oldValue <> value Then
                ViewState("DoPostBack") = value
            End If
        End Set
    End Property
    <Browsable(False)>
    Property ScrollTop As String
        Get
            If ViewState("ScrollTop") IsNot Nothing AndAlso ViewState("ScrollTop").ToString <> String.Empty Then
                Return ViewState("ScrollTop").ToString
            Else
                ViewState("ScrollTop") = "0"
                Return "0"
            End If
        End Get
        Set(value As String)
            Dim oldValue As String = String.Empty
            If ViewState("ScrollTop") IsNot Nothing Then _
                oldValue = ViewState("ScrollTop").ToString
            If value <> oldValue Then
                If value = String.Empty Then
                    ViewState("ScrollTop") = "0"
                Else
                    If IsNumeric(value) And Not value.Contains(".") Then
                        ViewState("ScrollTop") = value
                    Else
                        ViewState("ScrollTop") = "0"
                    End If
                End If
                hfScrollTop.Value = ViewState("ScrollTop").ToString

            End If
        End Set

    End Property
    <Browsable(False)>
    Property HasLoaded As String
        Get
            If ViewState("HasLoaded") IsNot Nothing AndAlso ViewState("HasLoaded").ToString <> String.Empty Then
                Return ViewState("HasLoaded").ToString
            Else
                ViewState("HasLoaded") = "0"
                Return "0"
            End If
        End Get
        Set(value As String)
            Dim oldValue As String = String.Empty
            If ViewState("HasLoaded") IsNot Nothing Then
                oldValue = ViewState("HasLoaded").ToString
            End If
            If value <> oldValue Then
                If value = String.Empty Then
                    ViewState("HasLoaded") = "0"
                Else
                    If IsNumeric(value) AndAlso Not value.Contains(".") Then
                        If value <> "0" Then value = "1"
                        ViewState("HasLoaded") = value
                    Else
                        ViewState("HasLoaded") = "0"
                    End If

                End If
            End If
        End Set
    End Property
    <Browsable(False)>
    Property IsFocused As String
        Get
            If ViewState("IsFocused") IsNot Nothing AndAlso ViewState("IsFocused").ToString <> String.Empty Then
                Return ViewState("IsFocused").ToString
            Else
                ViewState("IsFocused") = "0"
                Return "0"
            End If
        End Get
        Set(value As String)
            Dim oldValue As String = String.Empty
            If ViewState("IsFocused") IsNot Nothing Then
                oldValue = ViewState("IsFocused").ToString
            End If
            If value <> oldValue Then
                If value = String.Empty Then
                    ViewState("IsFocused") = "0"
                Else
                    If IsNumeric(value) AndAlso Not value.Contains(".") Then
                        If value <> "0" Then value = "1"
                        ViewState("IsFocused") = value
                    Else
                        ViewState("IsFocused") = "0"
                    End If
                End If
                hfIsFocused.Value = ViewState("IsFocused").ToString
            End If
        End Set
    End Property
    <Browsable(False)>
    Property SelectedIndex As String
        Get
            If DoPostBack Then
                If ViewState("SelectedIndex") IsNot Nothing AndAlso ViewState("SelectedIndex").ToString <> String.Empty Then
                    Return ViewState("SelectedIndex").ToString
                Else
                    ViewState("SelectedIndex") = "-1"
                    Return "-1"
                End If
            Else
                If hfSelectedIndex.Value = "" Then
                    hfSelectedIndex.Value = "-1"
                    Return "-1"
                Else
                    Return hfSelectedIndex.Value
                End If
            End If
        End Get
        Set(value As String)
            Dim oldIndex As String = String.Empty
            If DoPostBack Then
                If ViewState("SelectedIndex") IsNot Nothing Then _
                oldIndex = ViewState("SelectedIndex").ToString
            Else
                oldIndex = hfSelectedIndex.Value
            End If
            If value <> oldIndex Then
                If value = String.Empty Then
                    ViewState("SelectedIndex") = "-1"
                Else
                    If IsNumeric(value) AndAlso Not value.StartsWith("-") AndAlso Not value.Contains(".") Then
                        Dim val As Integer = CInt(value)
                        If val < DragItems.Items.Length Then
                            ViewState("SelectedIndex") = value
                        Else
                            ViewState("SelectedIndex") = "-1"
                        End If

                    Else
                        ViewState("SelectedIndex") = "-1"
                    End If
                End If
                hfSelectedIndex.Value = ViewState("SelectedIndex").ToString
                Dim index As Integer = CInt(ViewState("SelectedIndex").ToString)
                If index > -1 AndAlso index < DragItems.Items.Count Then
                    Dim di As DragItem = DragItems.Items(index)
                    Dim NewDI As New DragItem
                    NewDI.ID = di.ID
                    NewDI.Tag = di.Tag
                    NewDI.Text = di.Text

                    Dim arg As New DragListEventArgs(NewDI, index)
                    OnSelectedIndexChanged(arg)
                End If
                If Not Loading AndAlso HasLoaded = "1" Then
                    CreateListItems()
                    Refresh()
                End If
            End If

        End Set
    End Property
    <Browsable(False)>
    Public Property DragItems As DragListItems
        Get
            If DragListItemsData = String.Empty Then
                Return Nothing
            Else
                Return JsonConvert.DeserializeObject(Of DragListItems)(DragListItemsData)
            End If
        End Get
        Set(value As DragListItems)
            If value IsNot Nothing Then
                DragListItemsData = JsonConvert.SerializeObject(value)
            End If
        End Set
    End Property
    <Browsable(False)>
    Public Property DragListItemsData As String
        Get
            If ViewState("DragListItemsData") IsNot Nothing AndAlso ViewState("DragListItemsData").ToString <> String.Empty Then
                Return ViewState("DragListItemsData").ToString
            Else
                ViewState("DragListItemsData") = String.Empty
                Return String.Empty
            End If
        End Get
        Set(value As String)
            If value <> String.Empty AndAlso IsJson(value) Then
                ViewState("DragListItemsData") = value
            End If
        End Set
    End Property

#End Region

#Region "Public Methods/Functions"
    Public Sub Refresh()

        If Not Loading AndAlso HasLoaded = "1" Then
            hfDoPostBack.Value = "1"
        End If
    End Sub
    Public Sub AdjustScroll()
        hfAdjustScroll.Value = "1"
    End Sub
#End Region

#Region "Private Methods/Functions"

    Function IsJson(value As String) As Boolean
        Try
            Dim obj As Object = JsonConvert.DeserializeObject(value)
        Catch ex As JsonException
            Return False
        End Try
        Return True
    End Function
    Private Sub EmbedStyleSheet(ThisType As Type)
        If Not Me.DesignMode Then
            Dim cssURL As String = Page.ClientScript.GetWebResourceUrl(ThisType, ThisType.Namespace & ".DragList.css")
            Dim csm As ClientScriptManager = Page.ClientScript
            Dim cssLink As New HTC.HtmlLink
            cssLink.Href = cssURL
            cssLink.Attributes.Add("rel", "stylesheet")
            cssLink.Attributes.Add("type", "text/css")
            Dim ss As String = cssLink.ToString
            Page.Header.Controls.Add(cssLink)
            Dim smName = "cssDragList"
            If Not csm.IsClientScriptBlockRegistered(ThisType, smName) Then
                Dim css As String = "<link href=""" & cssURL & " type=""text/css"" rel=""stylesheet"" />"
                csm.RegisterClientScriptBlock(ThisType, smName, css, False)
            End If
        End If

    End Sub
    Private Sub EmbedJavaScript(ThisType As Type)
        If Not Me.DesignMode Then
            Dim csm As ClientScriptManager = Page.ClientScript
            Dim smName As String = "DragListScript"
            If Not csm.IsClientScriptBlockRegistered(ThisType, smName) Then
                Dim names As String() = ThisType.Assembly.GetManifestResourceNames
                If names.Length > 0 Then
                    Dim ThisAssembly As Assembly = ThisType.Assembly
                    Dim AssemblyName As String = ThisAssembly.GetName.Name
                    Dim JavaResource As String = ".DragList.js"
                    Dim exAssembly As Assembly = Assembly.GetExecutingAssembly
                    Dim st As Stream = Assembly.GetExecutingAssembly.GetManifestResourceStream(ThisType.Namespace & JavaResource)
                    Dim sr As StreamReader = New StreamReader(st)
                    csm.RegisterClientScriptBlock(ThisType, smName, sr.ReadToEnd, True)
                End If
            End If
        End If
    End Sub
    Private Sub CreateListItems()
        ulList.Controls.Clear()

        If DragItems IsNot Nothing AndAlso DragItems.Items.Length > 0 Then
            Dim di As DragItem = Nothing
            Dim li As HTC.HtmlGenericControl
            For i As Integer = 0 To DragItems.Items.Length - 1
                di = DragItems.Items(i)
                li = New HTC.HtmlGenericControl("li")
                li.EnableViewState = True
                li.ID = ClientID & "_li" & (i + 1).ToString
                li.InnerHtml = di.Text
                li.Attributes.Add("dragitemID", di.ID)
                li.Attributes.Add("dragitemTag", di.Tag)
                'li.Attributes.Add("runat", "server")
                'li.Attributes.Add("tabindex", (i + 1).ToString)
                li.Attributes.Add("tabindex", "-1")
                li.Attributes.Add("onfocus", "OnItemClick(""" & Me.ClientID & """);")
                li.Attributes.Add("ondblclick", "OnDblClick(""" & Me.ClientID & """);")
                li.Attributes.Add("class", "li")
                If Draggable Then
                    li.Attributes.Add("draggable", "true")
                    li.Attributes.Add("ondragstart", "drag(event);")
                    If DropOK Then
                        li.Attributes.Add("ondrop", "drop(event);")
                        li.Attributes.Add("ondragover", "allowDrop(event);")
                    Else

                    End If
                Else
                    li.Attributes.Add("draggable", "false")
                End If

                'li.Attributes.Add("onfocus", "OnItemFocus(""" & Me.ClientID & """);")
                'li.Attributes.Add("onblur", "OnItemBlur(""" & Me.ClientID & """);")
                li.Attributes.Add("index", i.ToString)
                li.Style("font") = Font.Name
                li.Style("font-size") = Font.Size.ToString
                'li.Style("color") = ForeColor.Name
                ulList.Controls.Add(li)
                If i.ToString = Me.SelectedIndex Then
                    li.Attributes.Add("class", "selected")
                End If
            Next

        End If

    End Sub
    Private Sub WindowOnLoadScript()
        Dim sb As New System.Text.StringBuilder("")
        Dim cs As ClientScriptManager = Page.ClientScript

        With sb
            .Append("<script language='JavaScript'>")
            .Append("window.onload = AdjustScrollPosition('" & ClientID & "');")
            .Append("var timerId=window.setInterval(checkPostBack,50,'" & ClientID & "');")
            .Append("</script>")
        End With
        cs.RegisterStartupScript(Me.GetType, ClientID, sb.ToString)

    End Sub
#End Region

#Region "Overrides"
    Protected Overrides Sub OnInit(e As EventArgs)
        MyBase.OnInit(e)
        EmbedStyleSheet(Me.GetType)
    End Sub
    Protected Overrides Sub RenderContents(ByVal writer As HtmlTextWriter)
        If DesignMode Then
            writer.Write(ClientID)
            Return
        End If
        MyBase.RenderContents(writer)
    End Sub
    Protected Overrides Sub AddAttributesToRender(writer As HtmlTextWriter)
        writer.AddAttribute("data-selectedindex", SelectedIndex)
        MyBase.AddAttributesToRender(writer)
    End Sub
    Protected Overrides Sub CreateChildControls()

        Me.Controls.Clear()

        'hidden fields
        hfSelectedIndex.ID = ClientID & "_hfSelectedIndex"
        hfSelectedIndex.Value = SelectedIndex
        Me.Controls.Add(hfSelectedIndex)
        hfScrollTop.ID = ClientID & "_hfScrollTop"
        hfScrollTop.Value = ScrollTop
        Me.Controls.Add(hfScrollTop)
        hfAutoPostBack.ID = ClientID & "_hfAutoPostBack"
        hfAutoPostBack.Value = DoPostBack.ToString.ToLower
        Me.Controls.Add(hfAutoPostBack)
        hfDoPostBack.ID = ClientID & "_hfDoPostBack"
        hfDoPostBack.Value = "0"
        Me.Controls.Add(hfDoPostBack)
        hfIsFocused.ID = ClientID & "_hfIsFocused"
        hfIsFocused.Value = IsFocused
        Me.Controls.Add(hfIsFocused)
        hfAdjustScroll.ID = ClientID & "_hfAdjustScroll"
        hfAdjustScroll.Value = "1"
        Me.Controls.Add(hfAdjustScroll)

        'content container
        Dim divcontent As New HTC.HtmlGenericControl("div")
        divcontent.Style("width") = "100%"
        divcontent.Style("height") = Height.ToString
        divcontent.ID = ClientID & "_Content"
        'heading
        Dim divHeading As New HTC.HtmlGenericControl("div")
        If HasHeader Then
            divHeading.ID = ClientID & "_Heading"
            divHeading.InnerHtml = Text
            Dim HeadBackground As String = HeadingBackColor.Name.ToLower
            Dim HeadColor As String = HeadingForeColor.Name.ToLower

            If Not HeadingBackColor.IsNamedColor Then
                Dim r As String = Hex(HeadingBackColor.R)
                Dim g As String = Hex(HeadingBackColor.G)
                Dim b As String = Hex(HeadingBackColor.B)

                r = IIf(r.Length = 1, "0" & r, r).ToString
                g = IIf(g.Length = 1, "0" & g, g).ToString
                b = IIf(b.Length = 1, "0" & b, b).ToString

                HeadBackground = "#" & r & g & b
            End If
            If Not HeadingForeColor.IsNamedColor Then
                Dim r As String = Hex(HeadingForeColor.R)
                Dim g As String = Hex(HeadingForeColor.G)
                Dim b As String = Hex(HeadingForeColor.B)

                r = IIf(r.Length = 1, "0" & r, r).ToString
                g = IIf(g.Length = 1, "0" & g, g).ToString
                b = IIf(b.Length = 1, "0" & b, b).ToString

                HeadColor = "#" & r & g & b

            End If
            divHeading.Style("background") = HeadBackground.ToLower
            divHeading.Style("color") = HeadColor.ToLower
            Select Case HeadingAlignment
                Case HeaderAlign.Center
                    divHeading.Style("text-align") = "center"
                Case HeaderAlign.Left
                    divHeading.Style("text-align") = "left"
                Case HeaderAlign.right
                    divHeading.Style("text-align") = "right"
            End Select
            divHeading.Style("height") = HeadingHeight.ToString
            divHeading.Style("line-height") = HeadingHeight.ToString
            divHeading.Style("width") = "100%"
            divHeading.Style("border-bottom") = "1px solid"
            divHeading.Style("border-bottom-color") = BorderColor.Name
            'add heading to content container
            divcontent.Controls.Add(divHeading)

        End If

        'list container
        Dim divUL As New HTC.HtmlGenericControl("div")
        divUL.ID = ClientID & "_ListContainer"
        divUL.Style("width") = "100%"

        If Height.Type = UnitType.Pixel Then
            If HasHeader Then
                divUL.Style("height") = CStr(Height.Value - Val(divHeading.Style("height")) - 12) & "px"
            Else
                divUL.Style("height") = CStr(Height.Value - 12) & "px"
            End If
        ElseIf Height.Type = UnitType.Percentage Then
            If HasHeader Then
                divUL.Style("Height") = "90%"
            Else
                divUL.Style("Height") = "100%"
            End If
        End If
        'ul
        ulList.EnableViewState = True
        ulList.ID = Me.ClientID & "_InnerList"
        ulList.Attributes.Add("runat", "server")
        ulList.Attributes.Add("class", "draglist")
        ulList.Style("padding") = "1px"
        ulList.Style("height") = "100%"
        ulList.Style("overflow-y") = "auto"
        ulList.Style("scrollbar-base-color") = "gray"

        'add list to list container
        divUL.Controls.Add(ulList)

        'add list container to content container
        divcontent.Controls.Add(divUL)
        'add content container to control
        Me.Controls.Add(divcontent)
        MyBase.CreateChildControls()

    End Sub
    Protected Overrides Sub OnPreRender(e As EventArgs)
        If Not Me.DesignMode Then
            EmbedJavaScript(Me.GetType)
        End If
        MyBase.OnPreRender(e)
    End Sub
    Protected Overrides ReadOnly Property tagkey As System.Web.UI.HtmlTextWriterTag
        Get
            Return HtmlTextWriterTag.Div
        End Get
    End Property

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        MyBase.Render(writer)
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        Loading = True
        HasLoaded = "0"
        Dim target As String = Page.Request("__EVENTTARGET")
        Dim data As String = Page.Request("__EVENTARGUMENT")

        If target IsNot Nothing AndAlso data IsNot Nothing AndAlso target = Me.ID AndAlso data.Contains("~") Then
            Dim params As String() = data.Split("~"c)
            ScrollTop = params(1)
            SelectedIndex = params(0)
            IsFocused = "1"
        End If
        CreateListItems()
        Loading = False
        HasLoaded = "1"
        MyBase.OnLoad(e)
        ulList.Attributes.Add("tabindex", "0")
        ulList.Attributes.Add("onkeydown", "OnItemNavigate(""" & Me.ClientID & """);")
        ulList.Attributes.Add("AutoPostBack", DoPostBack.ToString.ToLower)
        WindowOnLoadScript()
        'CreateCallBackEventHandler()
    End Sub
#End Region

#Region "ICallbackEventHandler Methods"
    'Private Sub CreateCallBackEventHandler()
    '    Dim cm As ClientScriptManager = Page.ClientScript
    '    Dim cbReference As String = cm.GetCallbackEventReference(Me, "arg", "ReceiveServerData", "context", True)
    '    Dim callbackScript As String
    '    callbackScript = "function CallServer" & ClientID.ToString & "(arg, context)" & "{" & cbReference & ";}"
    '    cm.RegisterClientScriptBlock(Me.GetType(), Me.ClientID, callbackScript, True)
    'End Sub
    'Public Sub RaiseCallbackEvent(arg As String) Implements ICallbackEventHandler.RaiseCallbackEvent
    '    Dim args As String() = arg.Split("_"c)
    '    Dim argid As String = args(1)
    '    If argid = ClientID Then
    '        SelectedIndex = args(0)
    '    End If

    'End Sub

    'Public Function GetCallbackResult() As String Implements ICallbackEventHandler.GetCallbackResult
    '    'Dim sw As New StringWriter
    '    'Dim tw As New HtmlTextWriter(sw)
    '    'Dim mi As System.Reflection.MethodInfo = Me.GetType.GetMethod("RenderContents", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic)
    '    'mi.Invoke(Me, New Object() {tw})
    '    'Return String.Format("{0}|{1}", ClientID, sw.ToString)
    '    Return String.Empty
    'End Function
#End Region

#Region "IScriptControl Implementations"
    'Public Function GetScriptDescriptors() As IEnumerable(Of ScriptDescriptor) Implements IScriptControl.GetScriptDescriptors
    '    Dim thistype = Me.GetType
    '    Dim descriptor As ScriptControlDescriptor = New ScriptControlDescriptor(Me.GetType.FullName, ClientID)
    '    descriptor.AddProperty("DragListItemsData", Me.DragListItemsData)
    '    Return New ScriptDescriptor() {descriptor}
    'End Function

    'Public Function GetScriptReferences() As IEnumerable(Of ScriptReference) Implements IScriptControl.GetScriptReferences
    '    Dim reference As New ScriptReference()
    '    'reference.Path = "Scripts/DragList.js"
    '    reference.Assembly = "WebControls"
    '    reference.Name = "MyWebControls.DragList.js"
    '    Return New ScriptReference() {reference}
    'End Function

#End Region

End Class
