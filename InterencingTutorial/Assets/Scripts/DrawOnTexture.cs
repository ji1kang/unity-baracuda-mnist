using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawOnTexture : MonoBehaviour
{
    public Texture2D baseTexture;
    
    // Update is called once per frame
    void Update()
    {
        DoMouseDrawing();
    }

    /// <summary>
    /// 마우스로 텍스쳐를 그립니다
    /// </summary>
    /// <exception cref="Exception"></exception>

    private void DoMouseDrawing()
    {
        // Don't bother trying to run if we can't find the main camera.
        if (Camera.main == null)
        {
            throw new Exception("연결된 카메라를 찾을 수 없습니다");
        }
        
        // 마우스 버튼 눌렸는지 확인
        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) return;
        // 유니티에서 Ray는 지정한 위치에서 무한대로 발사되는 선 (광선!) 
        // -> 이걸 이용하면 Ray에 닿은 물체 정보 반환가능
        // Cast a ray into the scene from screenspace where the mouse is.
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 마우스 작동 안할 때 확인
        // Do nothing if we aren't hitting anything.
        if (!Physics.Raycast(mouseRay, out hit)) return;
        // Do nothing if we didn't get hit.
        if (hit.collider.transform != transform) return;

        // 마우스 ray가 닿은 좌표 가져오기
        // Get the UV coordinate that the mouseRay hit
        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= baseTexture.width;
        pixelUV.y *= baseTexture.height;

        // 색깔 관련 작업
        // Set the color as white if the lmb is being pressed, black if rmb.
        Color colorToSet = Input.GetMouseButton(0) ? Color.white : Color.black;


        // Update the texture and apply.
        baseTexture.SetPixel((int)pixelUV.x, (int)pixelUV.y, colorToSet);
        baseTexture.Apply();

    }
}
