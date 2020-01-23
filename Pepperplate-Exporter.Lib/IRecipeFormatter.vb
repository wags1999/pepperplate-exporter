Public Interface IRecipeFormatter

    Sub WriteRecipe(recipe As PepperplateAPI.RecipeSync, path As String)

    ReadOnly Property FileExtension As String

End Interface
