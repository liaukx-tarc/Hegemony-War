using UnityEngine;

public class Area : Building
{
    public City belongCity;

    public void AreaTurn()
    {
        if(belongCell.unit == null || belongCell.unit.player == player)
        {
            if(currentHp < maxHP)
            {
                currentHp += WorldController.instance.buildingController.restoreHP;
                currentHp = Mathf.Min(currentHp, maxHP);
                slider.value = currentHp;

                if (currentHp == maxHP)
                {
                    slider.gameObject.SetActive(false);

                    if(isDestroy)
                    {
                        isDestroy = false;
                        belongCity.CalculateIncome();
                    }
                }
            }
        }
    }
}
