using System.Collections.Generic;
using UnityEngine;

public class RecipeBookManager : MonoBehaviour
{
    public static RecipeBookManager Instance;

    public List<RecipeData> savedRecipes = new List<RecipeData>();

    private void Awake()
    {
        Instance = this;
    }

    public void SaveRecipe(List<KeywordData> keywords, CraftedItemResult result)
    {
        RecipeData recipe = new RecipeData();

        recipe.recipeName = result.itemName;
        recipe.bestGrade = result.grade;

        for (int i = 0; i < keywords.Count; i++)
        {
            recipe.keywordNames.Add(keywords[i].keywordName);
        }

        savedRecipes.Add(recipe);

        Debug.Log("레시피 저장 완료: " + recipe.recipeName);
    }
}