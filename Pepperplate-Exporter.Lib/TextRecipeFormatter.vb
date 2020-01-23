Public Class TextRecipeFormatter
    Implements IRecipeFormatter

    Public ReadOnly Property FileExtension As String Implements IRecipeFormatter.FileExtension
        Get
            Return ".txt"
        End Get
    End Property

    Public Sub WriteRecipe(recipe As PepperplateAPI.RecipeSync, path As String) Implements IRecipeFormatter.WriteRecipe
        Using file = My.Computer.FileSystem.OpenTextFileWriter(path & FileExtension, False)
            file.WriteLine("Title: " & recipe.Title)
            file.WriteLine("Yield: " & recipe.Yield)
            file.WriteLine("Active Time: " & recipe.ActiveTime)
            file.WriteLine("Total Time: " & recipe.TotalTime)
            file.WriteLine("Categories: " & String.Join("; ", recipe.Tags.Select(Function(t) t.Text).ToArray))
            file.WriteLine("Description: " & recipe.Description)
            file.WriteLine("Favorite: " & recipe.Favorite)
            file.WriteLine("Source: " & recipe.Source)
            file.WriteLine("Manual Source: " & recipe.ManualSource)
            file.WriteLine("URL: " & recipe.Url)
            file.WriteLine("Note: " & recipe.Note)

            file.WriteLine()
            file.WriteLine("Ingredients:")
            For Each ingredientGroup In recipe.Ingredients.OrderBy(Function(i) i.DisplayOrder)
                If Not String.IsNullOrEmpty(ingredientGroup.Title) Then
                    file.WriteLine("[" & ingredientGroup.Title & "]")
                End If
                For Each ingredient In ingredientGroup.Ingredients.OrderBy(Function(i) i.DisplayOrder)
                    file.WriteLine(ingredient.Quantity & " " & ingredient.Text)
                Next
            Next

            file.WriteLine()
            file.WriteLine("Directions:")
            For Each directionGroup In recipe.Directions.OrderBy(Function(d) d.DisplayOrder)
                If Not String.IsNullOrEmpty(directionGroup.Title) Then
                    file.WriteLine("[" & directionGroup.Title & "]")
                End If
                For Each direction In directionGroup.Directions.OrderBy(Function(d) d.DisplayOrder)
                    file.WriteLine(direction.Text)
                Next
            Next

            If Not String.IsNullOrEmpty(recipe.ImageUrl) Then
                My.Computer.Network.DownloadFile(recipe.ImageUrl, path & ".jpg", Nothing, Nothing, False, 100000, True, FileIO.UICancelOption.DoNothing)
            End If
        End Using
    End Sub
End Class
