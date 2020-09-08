using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] Transform weaponHolder;
    [SerializeField] List<GunController> guns;
    [SerializeField] LayerMask gunLayer;
    int weaponIndex;
    Shooting shooting;
    private void Awake()
    {
        shooting = GetComponent<Shooting>();
    }
    public void Interact(Collider2D collider)
    {
        if (guns.Count > 0)
        {
            guns[weaponIndex].DroppedDown();
            collider.GetComponent<Cupgrade>().AddItem(guns[weaponIndex].gameObject);
            guns.RemoveAt(weaponIndex);
            SwitchUp();
        }
        else
        {
            Collider2D[] guns = Physics2D.OverlapCircleAll(transform.position, radius, gunLayer);
            if (guns.Length == 0)
                return;
            GameObject aGun = guns[0].gameObject;
            collider.GetComponent<Cupgrade>().RemoveItem(aGun);
            PickUpGun(aGun.GetComponent<GunController>());
        }
    }
    public void PickUpGun(GunController newGun)
    {
        newGun.PickedUp(GetComponent<Rigidbody2D>());
        guns.Add(newGun);
        GameManager.Instance.AddWeapon(newGun.name);
        if (newGun.transform.parent)
            if (newGun.transform.parent.GetComponent<Cupgrade>() != null)
                newGun.transform.parent.GetComponent<Cupgrade>().RemoveItem(newGun.gameObject);
        newGun.transform.parent = weaponHolder;
        newGun.transform.localPosition = Vector3.zero;
        newGun.transform.localRotation = Quaternion.identity;
        newGun.transform.localScale = new Vector3(Mathf.Abs(newGun.transform.localScale.x), Mathf.Abs(newGun.transform.localScale.y), Mathf.Abs(newGun.transform.localScale.z));
        if (shooting.GetWeaponHeld() == null)
            SwitchToWeapon(guns.Count - 1);
        else
            newGun.gameObject.SetActive(false);
        return;
    }

    public void SwitchToWeapon(int index)
    {
        if (guns[weaponIndex])
        {
            guns[weaponIndex].Switched();
            guns[weaponIndex].gameObject.SetActive(false);
        }
        weaponIndex = index;
        shooting.SetWeaponHeld(guns[weaponIndex]);
        if (guns[weaponIndex])
        {
            guns[weaponIndex].Switched();
            guns[weaponIndex].gameObject.SetActive(true);

            UIManager.Instance.SetWeaponUI(guns[weaponIndex].name, guns[weaponIndex].GetProjectileLeft(), guns[weaponIndex].GetMagazineSize(), guns[weaponIndex].GetWeaponImage());
        }
    }
    public void SwitchUp()
    {
        if (guns.Count > 0)
        {
            if (weaponIndex < guns.Count)
            {
                guns[weaponIndex].Switched();
                guns[weaponIndex].gameObject.SetActive(false);
            }
            weaponIndex++;
            if (weaponIndex > guns.Count - 1)
                weaponIndex = 0;

            shooting.SetWeaponHeld(guns[weaponIndex]);
            if (weaponIndex < guns.Count)
            {
                guns[weaponIndex].Switched();
                guns[weaponIndex].gameObject.SetActive(true);

                UIManager.Instance.SetWeaponUI(guns[weaponIndex].name, guns[weaponIndex].GetProjectileLeft(), guns[weaponIndex].GetMagazineSize(), guns[weaponIndex].GetWeaponImage());
            }
        }
        else
        {
            shooting.SetWeaponHeld(null);
        }

    }
    public void SwitchDown()
    {
        if (guns.Count > 0)
        {
            if (weaponIndex < guns.Count)
            {
                guns[weaponIndex].Switched();
                guns[weaponIndex].gameObject.SetActive(false);
            }
            weaponIndex--;
            if (weaponIndex < 0)
                weaponIndex = guns.Count - 1;
            shooting.SetWeaponHeld(guns[weaponIndex]);
            if (weaponIndex < guns.Count)
            {
                guns[weaponIndex].Switched();
                guns[weaponIndex].gameObject.SetActive(true);

                UIManager.Instance.SetWeaponUI(guns[weaponIndex].name, guns[weaponIndex].GetProjectileLeft(), guns[weaponIndex].GetMagazineSize(), guns[weaponIndex].GetWeaponImage());
            }
        }
        else
        {
            shooting.SetWeaponHeld(null);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
