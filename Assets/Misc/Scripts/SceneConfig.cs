using UnityEngine;

namespace DxrCrystal2 {

sealed class SceneConfig : MonoBehaviour
{
    [field:SerializeField] int FrameRate = 30;

    void Start() => Application.targetFrameRate = FrameRate;
}

} // namespace DxrCrystal2
