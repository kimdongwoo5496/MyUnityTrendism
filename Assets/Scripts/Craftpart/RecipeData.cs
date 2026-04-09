using System;
using System.Collections.Generic;

[Serializable]
public class RecipeData
{
    public string recipeName;
    public List<string> keywordNames = new List<string>();
    public string bestGrade;
}