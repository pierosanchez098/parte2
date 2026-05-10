Public Class UnidadItem
    Public Property Text As String
    Public Property Value As Integer

    Public Overrides Function ToString() As String
        Return Text
    End Function
End Class