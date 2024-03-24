
using System.Collections.Generic;

public class GlobalNZSave : BaseNZSave
{
    public PrefsSave Preferences = new();

    public List<SkinData.Data> SavedCharacters = new();
}
