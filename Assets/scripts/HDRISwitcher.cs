using UnityEngine;

public class HDRISwitcher : MonoBehaviour
{
    [Header("HDRI Settings")]
    [Tooltip("Array of HDRI textures to cycle through")]
    public Texture[] hdriTextures;
    
    [Header("Material Settings")]
    [Tooltip("Material of the inverted sphere (will be auto-assigned if not set)")]
    public Material sphereMaterial;
    
    // Private variables
    private int currentTextureIndex = 0;
    private Renderer sphereRenderer;
    
    void Start()
    {
        // Initialize components
        InitializeComponents();
        
        // Validate HDRI array
        if (hdriTextures == null || hdriTextures.Length == 0)
        {
            Debug.LogError("HDRISwitcher: No HDRI textures assigned! Please assign textures in the inspector.");
            return;
        }
        
        // Set initial HDRI texture
        SetHDRITexture(0);
    }
    
    void Update()
    {
        // Check for mouse click (works for both desktop and VR trigger)
        if (Input.GetMouseButtonDown(0))
        {
            NextHDRITexture();
        }
    }
    
    /// <summary>
    /// Initialize required components
    /// </summary>
    private void InitializeComponents()
    {
        // Get renderer component
        sphereRenderer = GetComponent<Renderer>();
        if (sphereRenderer == null)
        {
            Debug.LogError("HDRISwitcher: No Renderer component found on this GameObject!");
            return;
        }
        
        // Get or add collider for click detection
        Collider sphereCollider = GetComponent<Collider>();
        if (sphereCollider == null)
        {
            // Add sphere collider if none exists
            sphereCollider = gameObject.AddComponent<SphereCollider>();
            Debug.Log("HDRISwitcher: Added SphereCollider for interaction.");
        }
        
        // Get material
        if (sphereMaterial == null)
        {
            sphereMaterial = sphereRenderer.material;
        }
    }
    
    /// <summary>
    /// Switch to the next HDRI texture in the array
    /// </summary>
    public void NextHDRITexture()
    {
        if (hdriTextures == null || hdriTextures.Length == 0) return;
        
        currentTextureIndex = (currentTextureIndex + 1) % hdriTextures.Length;
        SetHDRITexture(currentTextureIndex);
        
        Debug.Log($"HDRISwitcher: Switched to HDRI texture {currentTextureIndex + 1}/{hdriTextures.Length}");
    }
    
    /// <summary>
    /// Switch to the previous HDRI texture in the array
    /// </summary>
    public void PreviousHDRITexture()
    {
        if (hdriTextures == null || hdriTextures.Length == 0) return;
        
        currentTextureIndex = (currentTextureIndex - 1 + hdriTextures.Length) % hdriTextures.Length;
        SetHDRITexture(currentTextureIndex);
        
        Debug.Log($"HDRISwitcher: Switched to HDRI texture {currentTextureIndex + 1}/{hdriTextures.Length}");
    }
    
    /// <summary>
    /// Set specific HDRI texture by index
    /// </summary>
    /// <param name="index">Index of the texture in the array</param>
    public void SetHDRITexture(int index)
    {
        if (hdriTextures == null || hdriTextures.Length == 0)
        {
            Debug.LogWarning("HDRISwitcher: No HDRI textures available!");
            return;
        }
        
        if (index < 0 || index >= hdriTextures.Length)
        {
            Debug.LogWarning($"HDRISwitcher: Index {index} is out of range! Array has {hdriTextures.Length} textures.");
            return;
        }
        
        if (hdriTextures[index] == null)
        {
            Debug.LogWarning($"HDRISwitcher: HDRI texture at index {index} is null!");
            return;
        }
        
        if (sphereMaterial == null)
        {
            Debug.LogError("HDRISwitcher: Sphere material is null!");
            return;
        }
        
        // Apply the texture to the material - Focus on _BaseMap first
        bool textureSet = false;
        
        // Try _BaseMap first (URP/HDRP)
        if (sphereMaterial.HasProperty("_BaseMap"))
        {
            sphereMaterial.SetTexture("_BaseMap", hdriTextures[index]);
            textureSet = true;
            Debug.Log($"HDRISwitcher: Set _BaseMap to texture {index}");
        }
        // Try _MainTex (Standard shaders)
        else if (sphereMaterial.HasProperty("_MainTex"))
        {
            sphereMaterial.mainTexture = hdriTextures[index];
            textureSet = true;
            Debug.Log($"HDRISwitcher: Set _MainTex to texture {index}");
        }
        // Try _EmissionMap (For emissive materials)
        else if (sphereMaterial.HasProperty("_EmissionMap"))
        {
            sphereMaterial.SetTexture("_EmissionMap", hdriTextures[index]);
            textureSet = true;
            Debug.Log($"HDRISwitcher: Set _EmissionMap to texture {index}");
        }
        
        if (!textureSet)
        {
            Debug.LogWarning("HDRISwitcher: Could not find suitable texture property on material!");
            Debug.Log($"Material shader: {sphereMaterial.shader.name}");
        }
        
        currentTextureIndex = index;
    }
    
    /// <summary>
    /// Get the current texture index
    /// </summary>
    /// <returns>Current texture index</returns>
    public int GetCurrentTextureIndex()
    {
        return currentTextureIndex;
    }
    
    /// <summary>
    /// Get the current HDRI texture
    /// </summary>
    /// <returns>Currently active HDRI texture</returns>
    public Texture GetCurrentTexture()
    {
        if (hdriTextures != null && currentTextureIndex < hdriTextures.Length)
        {
            return hdriTextures[currentTextureIndex];
        }
        return null;
    }
    
    // Simple mouse/VR trigger detection - works for both!
    void OnMouseDown()
    {
        NextHDRITexture();
    }
}