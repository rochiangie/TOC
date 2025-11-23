using UnityEngine;

public class PlayerInventoryVisuals : MonoBehaviour
{
    public GameObject bagModel;

    private void Start()
    {
        if (bagModel != null) bagModel.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.OnBagFilled += ShowBag;
        GameEvents.OnBagDisposed += HideBag;
    }

    private void OnDisable()
    {
        GameEvents.OnBagFilled -= ShowBag;
        GameEvents.OnBagDisposed -= HideBag;
    }

    private void ShowBag()
    {
        if (bagModel != null) bagModel.SetActive(true);
    }

    private void HideBag()
    {
        if (bagModel != null) bagModel.SetActive(false);
    }
}
