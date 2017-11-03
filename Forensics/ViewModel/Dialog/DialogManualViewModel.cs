using Forensics.Model.Device;
using Forensics.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.ViewModel.Dialog
{
    public class DialogManualViewModel : ViewModelBase
    {
        public override Pages PageIndex => throw new NotImplementedException();

        private string mstrConnection = ConfigurationManager.ConnectionStrings["mdb_phone"].ToString();

        private List<PhoneInfo> mlistPhoneAll = new List<PhoneInfo>();
        private List<BrandInfo> mlistBrandAll = new List<BrandInfo>();

        public List<PhoneInfo> listPhone { get; set; } = new List<PhoneInfo>();
        public List<BrandInfo> listBrand { get; set; } = new List<BrandInfo>();

        /// <summary>
        /// 搜索
        /// </summary>
        private string _textSearch;
        public string TextSearch
        {
            get
            {
                return _textSearch;
            }
            set
            {
                _textSearch = value;
                getPhones(_textSearch);
            }
        }

        // 品牌
        private int _selectedIndexBrand = -1;
        public int SelectedIndexBrand
        {
            get
            {
                return _selectedIndexBrand;
            }
            set
            {
                _selectedIndexBrand = value;

                BrandInfo br = this.listBrand[_selectedIndexBrand];
                var phones = mlistPhoneAll.Where(x => x.BrandName == br.Name).ToList();

                this.listPhone = phones.ToList();
                PropertyChanging("listPhone");
            }
        }

        // 手机
        private int _selectedIndexPhone = -1;
        public int SelectedIndexPhone
        {
            get
            {
                return _selectedIndexPhone;
            }
            set
            {
                _selectedIndexPhone = value;
            }
        }

        public DialogManualViewModel()
        {
            getPhones(null);
        }

        /// <summary>
        /// 获取机型
        /// </summary>
        /// <param name="keyword"></param>
        private void getPhones(string keyword)
        {
            mlistBrandAll.Clear();
            mlistPhoneAll.Clear();

            string strQuery = "select BrandModelID,BrandName,BrandNameEn,Model,ModelName,MobileVersion,AndroidVersion,PIC from edec_support where MarketYear <> \"" + "" + "\" ";
            if (!String.IsNullOrEmpty(keyword))
            {
                strQuery += "and BrandName like \"%" + keyword + "%\" or ModelName like \"%" + keyword + "%\" or Model like \"%" + keyword + "%\"";
            }

            DataTable dt = DatabaseUtil.Query(strQuery, mstrConnection);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow tmpdr in dt.Rows)
                {
                    var strBrandName = tmpdr["BrandName"].ToString();

                    // 添加Phone
                    PhoneInfo newPhone = new PhoneInfo()
                    {
                        BrandModelID = tmpdr["BrandModelID"].ToString(),
                        BrandName = tmpdr["BrandName"].ToString(),
                        ImagePath = tmpdr["PIC"].ToString(),
                        ModelName = tmpdr["ModelName"].ToString(),
                        BrandNameEn = tmpdr["BrandNameEn"].ToString()
                    };
                    mlistPhoneAll.Add(newPhone);

                    // 添加Brand
                    var bi = mlistBrandAll.Where(x => x.Name == strBrandName).FirstOrDefault();
                    if (bi == null)
                    {
                        BrandInfo newBrand = new BrandInfo()
                        {
                            Name = strBrandName,
                            NameEn = tmpdr["BrandNameEn"].ToString()
                        };
                        mlistBrandAll.Add(newBrand);
                    }
                }
            }

            this.listPhone = mlistPhoneAll.ToList();
            this.listBrand = mlistBrandAll.ToList();

            PropertyChanging("listPhone");
            PropertyChanging("listBrand");
        }
    }
}
