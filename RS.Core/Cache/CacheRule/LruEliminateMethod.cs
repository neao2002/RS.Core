using System.Collections;
using System;

namespace RS.Cache
{
	/// <summary>
	/// ���������̭�㷨
	/// </summary>
    [Serializable]
    public class LruEliminateMethod:IEliminateMethod
	{
		#region IEliminateMethod ��Ա
		/// <summary>
		/// Ȩֵ���ձ�
		/// </summary>
        private ArrayList MapOfWeights;
		public LruEliminateMethod()
		{
           MapOfWeights=new ArrayList();
		}
		

		/// <summary>
		/// ɾ����СȨֵ
		/// </summary>
		public void DelPoorWeight()
		{
			lock(MapOfWeights)
			{
				if(MapOfWeights.Count==0)return ;
				MapOfWeights.Remove(MapOfWeights[0]);
			}
		}
        
		/// <summary>
		/// ɾ��ĳ��Ȩֵ
		/// </summary>
		/// <param name="id"></param>

		public void DelWeight(object id)
		{
			lock(MapOfWeights)
			{
				MapOfWeights.Remove(id);
			}

		}

		/// <summary>
		/// ���Ȩֵ��С�����ID
		/// </summary>
		/// <returns></returns>
		public object GetPoorWeightID()
		{
			// TODO:  ��� LruEliminateMethod.GetPoorWeightID ʵ��
			lock(MapOfWeights)
			{
				if(MapOfWeights.Count==0)return null;
				return  MapOfWeights[0];
			}
		}

		/// <summary>
		/// ���һ��Ȩֵ��¼
		/// </summary>
		/// <param name="id"></param>
		public void InsertWeight(object id)
		{
			// TODO:  ��� LruEliminateMethod.InsertWeight ʵ��
			lock(MapOfWeights)
			{
				MapOfWeights.Add(id);
			}
		}

		

		/// <summary>
		/// �޸�Ȩֵ��С
		/// </summary>
		/// <param name="id"></param>
		public void UpdateWeight(object id)
		{
			// TODO:  ��� LruEliminateMethod.UpdateWeight ʵ��
			lock(MapOfWeights)
			{
				MapOfWeights.Remove(id);
				MapOfWeights.Add(id);
			}
		}

		#endregion

	}


}
