Imports System.IO
Imports System.Net
Imports System.Text.RegularExpressions

Public Class PepperplateExporter

    Private _recipeFormatter As IRecipeFormatter

    Public Sub New(formatter As IRecipeFormatter)
        _recipeFormatter = formatter
    End Sub

    Public Function ExportRecipes(username As String, password As String, folder As String, statusCallback As Action(Of String)) As Integer

        Dim pp As New PepperplateAPI.SyncManager With {.Url = "https://www.pepperplate.com/services/syncmanager5.asmx"}

        'login
        Dim loginResult = pp.GenerateLoginToken(username, password)
        If loginResult.Status <> PepperplateAPI.LoginTokenStatus.Success Then
            Throw New Exception("Error logging into Pepperplate.  Please check your username and password.")
        End If
        statusCallback("Successfully logged in...")

        'get recipe count
        Dim recipeCountResult = pp.HasUpdates(loginResult.Token, String.Empty, String.Empty, String.Empty)
        statusCallback($"{recipeCountResult.RecipeCount} total recipes to download...")

        'create output directory
        If Not My.Computer.FileSystem.DirectoryExists(folder) Then
            My.Computer.FileSystem.CreateDirectory(folder)
        End If

        'download and write recipe
        Dim recipeCount = 0
        Dim syncToken = String.Empty
        Dim recipesRemaining = 1
        Do While recipesRemaining > 0
            Dim recipeList = pp.RetrieveRecipes(loginResult.Token, syncToken, 10)
            For Each recipe In recipeList.Items
                Dim recipepath = Path.Combine(folder, CleanFileName(recipe.Title))
                _recipeFormatter.WriteRecipe(recipe, recipepath)
                recipeCount += 1
            Next
            syncToken = recipeList.SynchronizationToken
            recipesRemaining = recipeList.Remaining
            statusCallback($"Recipes downloaded... {recipesRemaining} remaining...")
        Loop

        Return recipeCount
    End Function


    Public Function CleanFileName(s As String) As String
        Dim regexSearch As String = Path.GetInvalidFileNameChars() & Path.GetInvalidPathChars()
        Dim r As New Regex(String.Format("[{0}]", Regex.Escape(regexSearch)))
        Return r.Replace(s, "")
    End Function

End Class
