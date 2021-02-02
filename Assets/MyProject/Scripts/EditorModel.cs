using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class EditorModel : MonoBehaviour
{
    // Start is called before the first frame update

    

    public RoleInfo Info;

    public Text RoleName;

    public Button HatAddBtn;
    public Button HatReduceBtn;
    public Text HatValue;

    public Button BodyAddBtn;
    public Button BodyReduceBtn;
    public Text BodyValue;

    public Button ArmAddBtn;
    public Button ArmReduceBtn;
    public Text ArmValue;

    public Button LegAddBtn;
    public Button LegReduceBtn;
    public Text LegValue;

    public Button VertalcalAddBtn;
    public Button VertalcalReduceBtn;
    public Text VertalcalValue;

    public Button FarwardAddBtn;
    public Button FarwardReduceBtn;
    public Text FarwardValue;


    public Button saveButton;

    public event Action UpdateEvent;

    void Start()
    {
      HatAddBtn.onClick.AddListener((() =>
      {
          Info.HatHeight += 0.01f;
          if (UpdateEvent != null) UpdateEvent();
          HatValue.text = Info.HatHeight.ToString(CultureInfo.InvariantCulture);

      }));
      HatReduceBtn.onClick.AddListener((() =>
      {
          Info.HatHeight -= 0.01f;
          if (UpdateEvent != null) UpdateEvent();
          HatValue.text = Info.HatHeight.ToString(CultureInfo.InvariantCulture);
      })); 
      



      BodyAddBtn.onClick.AddListener((() =>
      {
          Info.BodyScale += 0.01f;
          if (UpdateEvent != null) UpdateEvent();
          BodyValue.text = Info.BodyScale.ToString(CultureInfo.InvariantCulture);
      })); 
      BodyReduceBtn.onClick.AddListener((() =>
      {
          Info.BodyScale -= 0.01f;
          if (UpdateEvent != null) UpdateEvent();
          BodyValue.text = Info.BodyScale.ToString(CultureInfo.InvariantCulture);
      })); 
      



      ArmAddBtn.onClick.AddListener((() =>
      {
          Info.ArmScale += 0.01f;
          if (UpdateEvent != null) UpdateEvent();
          ArmValue.text = Info.ArmScale.ToString(CultureInfo.InvariantCulture); ;
      }));
      ArmReduceBtn.onClick.AddListener((() =>
      {
          Info.ArmScale -= 0.01f;
          if (UpdateEvent != null) UpdateEvent();
          ArmValue.text = Info.ArmScale.ToString(CultureInfo.InvariantCulture); ;
      })); 
     



      LegAddBtn.onClick.AddListener((() =>
      {
          Info.LegScale += 0.01f;
          if (UpdateEvent != null) UpdateEvent();
          LegValue.text = Info.LegScale.ToString(CultureInfo.InvariantCulture);
      })); 
      LegReduceBtn.onClick.AddListener((() =>
      {
          Info.LegScale -= 0.01f;
          if (UpdateEvent != null) UpdateEvent();
          LegValue.text = Info.LegScale.ToString(CultureInfo.InvariantCulture);
      })); 
      


      VertalcalAddBtn.onClick.AddListener((() =>
      {
          Info.Vertacal += 0.01f;
          if (UpdateEvent != null) UpdateEvent();
          VertalcalValue.text = Info.Vertacal.ToString(CultureInfo.InvariantCulture);
      })); 
      VertalcalReduceBtn.onClick.AddListener((() =>
      {
          Info.Vertacal -= 0.01f;
          if (UpdateEvent != null) UpdateEvent();
          VertalcalValue.text = Info.Vertacal.ToString(CultureInfo.InvariantCulture);
      })); 
     



      FarwardAddBtn.onClick.AddListener((() =>
      {
          Info.Farward -= 0.01f;
          if (UpdateEvent != null) UpdateEvent();
          FarwardValue.text = Info.Farward.ToString(CultureInfo.InvariantCulture); ;
      })); ;
      FarwardReduceBtn.onClick.AddListener((() =>
        {
            Info.Farward -= 0.01f;
            if (UpdateEvent != null) UpdateEvent();
            FarwardValue.text = Info.Farward.ToString(CultureInfo.InvariantCulture); ;
        })); ;

      saveButton.onClick.AddListener((() =>
      {
          GlobalSettings.SaveRoleInfo(Info);
      }));


    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetInfo(RoleInfo info)
    {
        Info = info;
        RoleName.text = info.Name;

        FarwardValue.text = Info.Farward.ToString(CultureInfo.InvariantCulture);
        VertalcalValue.text = Info.Vertacal.ToString(CultureInfo.InvariantCulture); ;
        LegValue.text = Info.LegScale.ToString(CultureInfo.InvariantCulture);
        ArmValue.text = Info.ArmScale.ToString(CultureInfo.InvariantCulture); ;
        BodyValue.text = Info.BodyScale.ToString(CultureInfo.InvariantCulture);
        HatValue.text = Info.HatHeight.ToString(CultureInfo.InvariantCulture);

    }
}
