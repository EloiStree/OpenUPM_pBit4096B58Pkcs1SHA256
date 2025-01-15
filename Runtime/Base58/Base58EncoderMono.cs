using System.Text;
using UnityEngine;

public class Base58EncoderMono : MonoBehaviour
{

    public string m_from;
    public string m_base58Encoded;
    public string m_to;

    private void OnValidate()
    {
        m_base58Encoded= Base58Encoder.Base58EncodeToUTF8(m_from);
        m_to = Base58Encoder.Base58DecodeFromUTF8(m_base58Encoded);

    }
}


