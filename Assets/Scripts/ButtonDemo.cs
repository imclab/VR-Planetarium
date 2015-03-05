using UnityEngine;
using System.Collections;
using LMWidgets;

public class ButtonDemo : ButtonBase {

  public ButtonDemoGraphics moverGraphics;
  public ButtonDemoGraphics botGraphics;
  
  public Color MoverGraphicsColor = new Color(0.0f, 0.5f, 0.5f, 1.0f);
  public Color BotGraphicsColor = new Color(0.0f, 1.0f, 1.0f, 1.0f);

  
  private void UpdateGraphics()
  {
    Vector3 position = transform.localPosition;
    position.z = Mathf.Min(position.z, m_localTriggerDistance);
    Vector3 bot_position = position;
    bot_position.z = Mathf.Max(bot_position.z, m_localTriggerDistance - m_localCushionThickness);
    botGraphics.transform.localPosition = bot_position;
    Vector3 mover_position = position;
    mover_position.z = (position.z + bot_position.z) / 2.0f;
    moverGraphics.transform.localPosition = mover_position;
  }
  
  protected override void Start()
  {
    base.Start();
  }
  
  protected override void FixedUpdate()
  {
    base.FixedUpdate();
    UpdateGraphics();
  }
}
