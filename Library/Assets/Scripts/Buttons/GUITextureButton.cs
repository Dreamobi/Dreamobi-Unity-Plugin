using UnityEngine;
using System.Collections;

/// <summary>
/// This is a class created purely for making GUITextures into a button.
/// It was developed with the intention of being used for touch devices, but will support hover textures if the dontAllowHover property is set to false.
/// Normally there would be a delegate function created to be performed in the PerformButtonPressLogic method, but because this is a simple example that has been ignored.
/// </summary>
/// <value name="downTexture">This is the texture used for the down state of the button</value>
/// <value name="hoverTexture">This is the texture used for the hover state of the button</value>
/// <value name="upTexture">This is the texture used for the up state of the button</value>
/// <value name="buttonGUITexture">This is a reference to the GUITexture component that is contained in the game object that this (GUITextureButton.cs) us attached to. It's mainly done so this.gameObject.GetComponent<GUITexture>() doesn't need to be called a lot of times.</value>
/// <value name="isPressed">This tracks the pressed state for the button</value>
/// <value name="dontAllowHover">This specifies for the button that the hover state should be ignored. In doing so even if the hover texture is set you will not see the button change to it on hover.</value>
/// <value name="isHovering">This tracks the hovering state for the button</value>
///
public class GUITextureButton : MonoBehaviour {

  //---------------------------------------------------------------------------
  //private
  //---------------------------------------------------------------------------
  GUITexture buttonGUITexture = null;
  //---------------------------------------------------------------------------
  //public
  //---------------------------------------------------------------------------
  public Texture downTexture = null;
  public Texture hoverTexture = null;
  public Texture upTexture = null;

  public bool isPressed = false;
  public bool dontAllowHover = false;
  public bool isHovering = false;
  //---------------------------------------------------------------------------

	public virtual void Start () {
    buttonGUITexture = this.gameObject.GetComponent<GUITexture>();
    buttonGUITexture.texture = upTexture;
	}

	void Update () {
	}

  void OnMouseEnter() {
    if(!dontAllowHover) {
      isHovering = true;
    }

    if(isPressed) {
      buttonGUITexture.texture = downTexture;
    }
    else if(isHovering) {
      buttonGUITexture.texture = hoverTexture;
    }
  }

  void OnMouseOver() {
    if(!dontAllowHover) {
      isHovering = true;
    }

    if(isPressed) {
      buttonGUITexture.texture = downTexture;
    }
    else if(isHovering) {
      buttonGUITexture.texture = hoverTexture;
    }
  }

  void OnMouseExit() {
    isHovering = false;
    buttonGUITexture.texture = upTexture;
  }

  void OnMouseDown() {
    isPressed = true;
    buttonGUITexture.texture = downTexture;
  }

  void OnMouseUpAsButton() {
    isPressed = false;
    if(dontAllowHover) {
      if(isHovering) {
        buttonGUITexture.texture = hoverTexture;
      }
      else {
        buttonGUITexture.texture = upTexture;
      }
    }
    PerformButtonPressLogic();
  }

  /// <summary>
  /// This is the method that should be called for button presses.
  /// </summary>
  public virtual void PerformButtonPressLogic() {
  }
}
