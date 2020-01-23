Imports System.Xml.Serialization

Public Class PPXMLFormatter
    Implements IRecipeFormatter

    Public ReadOnly Property FileExtension As String Implements IRecipeFormatter.FileExtension
        Get
            Return ".xml"
        End Get
    End Property

    Public Sub WriteRecipe(recipe As PepperplateAPI.RecipeSync, path As String) Implements IRecipeFormatter.WriteRecipe
        Dim x As XmlSerializer = New XmlSerializer(recipe.GetType())
        Using file = My.Computer.FileSystem.OpenTextFileWriter(path & FileExtension, False)
            x.Serialize(file, recipe)
        End Using

        Try
            If Not String.IsNullOrEmpty(recipe.ImageUrl) Then
                My.Computer.Network.DownloadFile(recipe.ImageUrl, path & ".jpg", Nothing, Nothing, False, 100000, True, FileIO.UICancelOption.DoNothing)
            End If
        Catch ex As Exception
            Console.WriteLine($"WARN: Could not download recipe image from URL {recipe.ImageUrl}; {ex.Message}")
        End Try

    End Sub


End Class
