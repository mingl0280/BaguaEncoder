using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;
using System.Security.Cryptography;
using static System.Text.Encoding;
using System.IO.Compression;
using System.IO;

namespace SuperEncoder
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }
        private Dictionary<string, char> ReplacementCipherCodeRevert = new Dictionary<string, char>() {
            {"乾",'A'},
            {"坤",'B'},
            {"屯",'C'},
            {"蒙",'D'},
            {"需",'E'},
            {"讼",'F'},
            {"师",'G'},
            {"比",'H'},
            {"小畜",'I'},
            {"履",'J'},
            {"泰",'K'},
            {"否",'L'},
            {"同人",'M'},
            {"大有",'N'},
            {"谦",'O'},
            {"豫",'P'},
            {"随",'Q'},
            {"蛊",'R'},
            {"临",'S'},
            {"观",'T'},
            {"噬嗑",'U'},
            {"贲",'V'},
            {"剥",'W'},
            {"复",'X'},
            {"无妄",'Y'},
            {"大畜",'Z'},
            {"颐",'a'},
            {"大过",'b'},
            {"坎",'c'},
            {"离",'d'},
            {"咸",'e'},
            {"恒",'f'},
            {"遁",'g'},
            {"大壮",'h'},
            {"晋",'i'},
            {"明夷",'j'},
            {"家人",'k'},
            {"睽",'l'},
            {"蹇",'m'},
            {"解",'n'},
            {"损",'o'},
            {"益",'p'},
            {"夬",'q'},
            {"姤",'r'},
            {"萃",'s'},
            {"升",'t'},
            {"困",'u'},
            {"井",'v'},
            {"革",'w'},
            {"鼎",'x'},
            {"震",'y'},
            {"艮",'z'},
            {"渐",'0'},
            {"归妹",'1'},
            {"丰",'2'},
            {"旅",'3'},
            {"巽",'4'},
            {"兑",'5'},
            {"涣",'6'},
            {"节",'7'},
            {"中孚",'8'},
            {"小过",'9'},
            {"既济",'+'},
            {"未济",'/'},
            {"大锴",'='}
        };

        private Dictionary<char, string> ReplacementCipherCodeForward = new Dictionary<char, string>() {
            {'A',"乾"},
            {'B',"坤"},
            {'C',"屯"},
            {'D',"蒙"},
            {'E',"需"},
            {'F',"讼"},
            {'G',"师"},
            {'H',"比"},
            {'I',"小畜"},
            {'J',"履"},
            {'K',"泰"},
            {'L',"否"},
            {'M',"同人"},
            {'N',"大有"},
            {'O',"谦"},
            {'P',"豫"},
            {'Q',"随"},
            {'R',"蛊"},
            {'S',"临"},
            {'T',"观"},
            {'U',"噬嗑"},
            {'V',"贲"},
            {'W',"剥"},
            {'X',"复"},
            {'Y',"无妄"},
            {'Z',"大畜"},
            {'a',"颐"},
            {'b',"大过"},
            {'c',"坎"},
            {'d',"离"},
            {'e',"咸"},
            {'f',"恒"},
            {'g',"遁"},
            {'h',"大壮"},
            {'i',"晋"},
            {'j',"明夷"},
            {'k',"家人"},
            {'l',"睽"},
            {'m',"蹇"},
            {'n',"解"},
            {'o',"损"},
            {'p',"益"},
            {'q',"夬"},
            {'r',"姤"},
            {'s',"萃"},
            {'t',"升"},
            {'u',"困"},
            {'v',"井"},
            {'w',"革"},
            {'x',"鼎"},
            {'y',"震"},
            {'z',"艮"},
            {'0',"渐"},
            {'1',"归妹"},
            {'2',"丰"},
            {'3',"旅"},
            {'4',"巽"},
            {'5',"兑"},
            {'6',"涣"},
            {'7',"节"},
            {'8',"中孚"},
            {'9',"小过"},
            {'+',"既济"},
            {'/',"未济"},
            {'=',"大锴"}
         };

        private void FormMain_Load(object sender, EventArgs e)
        {
            
        }

        private void buttonEncrypt_Click(object sender, EventArgs e)
        {
            if (richTextBoxClearText.Text == "")
                return;
            string clrTxt = richTextBoxClearText.Text;
            var txtBytes = UTF8.GetBytes(clrTxt);
            var deflatedBytes = Compress(txtBytes);
            var paddedBytes = AddRandomPadding(deflatedBytes);            
            var txtEncrypted = System.Convert.ToBase64String(paddedBytes);
            var txtReplaced = string.Join("",txtEncrypted.Select(f =>
            {
                return ReplacementCipherCodeForward[f];
            }));
            richTextBoxCryptogram.Text = txtReplaced;
        }

        RNGCryptoServiceProvider RProvider = new RNGCryptoServiceProvider();

        private byte[] AddRandomPadding(byte[] rawbytes)
        {
            byte[] paddedBytes = new byte[rawbytes.Length * 2];
            byte[] padByte = new byte[1];
            for(int i=0;i<rawbytes.Length;i++)
            {
                paddedBytes[i * 2 + (1 - (i % 2))] = rawbytes[i];
                RProvider.GetBytes(padByte);
                paddedBytes[i * 2 + (i % 2)] = padByte[0];
            }
            return paddedBytes;
        }

        private byte[] Compress(byte[] input)
        {
            byte[] outputBytes;
            MemoryStream ms = new MemoryStream();
            DeflateStream ds = new DeflateStream(ms, CompressionLevel.Optimal);
            ds.Write(input, 0, input.Length);
            ds.Close();
            outputBytes = ms.ToArray();
            return outputBytes;
        }

        private byte[] Decompress(byte[] input)
        {
            var ms = new MemoryStream();
            var ds = new DeflateStream(new MemoryStream(input), CompressionMode.Decompress);
            var buffer = new byte[4096];
            while(true)
            {
                var readLen = ds.Read(buffer, 0, 4096);
                if (readLen > 0)
                {
                    ms.Write(buffer, 0, readLen);
                }
                else
                {
                    break;
                }
            }
            ds.Close();
            return ms.ToArray();
        }

        private byte[] StripRandomPadding(byte[] encryptedBytes)
        {
            if (encryptedBytes.Length % 2 != 0)
            {
                MessageBox.Show("输入不正确！长度不是2的倍数", "哦呜", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            byte[] rawBytes = new byte[encryptedBytes.Length / 2];
            for(int i=0;i<encryptedBytes.Length/2;i++)
            {
                rawBytes[i] = encryptedBytes[2 * i + (1 - (i % 2))];
            }
            return rawBytes;
        }

        private void buttonDecrypt_Click(object sender, EventArgs e)
        {
            var cryptTxt = richTextBoxCryptogram.Text;
            string strNotReplaced = cryptTxt;
            foreach(var item in ReplacementCipherCodeRevert)
            {
                strNotReplaced = strNotReplaced.Replace(item.Key, item.Value.ToString());
            }
            byte[] dataUnencrypted = System.Convert.FromBase64String(strNotReplaced);
            byte[] dataNotPadded = StripRandomPadding(dataUnencrypted);
            byte[] dataDecompressed = Decompress(dataNotPadded);
            string clearText = UTF8.GetString(dataDecompressed);
            richTextBoxClearText.Text = clearText;
        }
    }
}
