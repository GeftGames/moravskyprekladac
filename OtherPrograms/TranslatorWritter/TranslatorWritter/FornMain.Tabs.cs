using System;
using System.Collections.Generic;

namespace TranslatorWritter {
    public partial class FormMain : System.Windows.Forms.Form{
                   
        #region Sentence
        void ListBoxSentences_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentSentence();
            
            int index=listBoxSentence.SelectedIndex;
            if (itemsSentences.Count==0) {
                SetNoneSentence();
                return;
            }
            if (index>=itemsSentences.Count) 
                index=itemsSentences.Count-1;
            if (index<0)
                index=0;
           
            CurrentSentence=itemsSentences[index];
            SetCurrentSentence();
            SetListBoxSentence();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void buttonSentenceAdd_Click(object sender, EventArgs e) {
            AddNewItemSentence();
        }

        void buttonSentenceRemove_Click(object sender, EventArgs e) {
            RemoveItemSentence(CurrentSentence);
            TextBoxSentenceFilter_TextChanged(null, new EventArgs());
        }
        
        void TextBoxSentenceFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentSentence();

            // Získej aktuální prvek
            ItemSentence selectedId=null;
            if (listBoxSentence.SelectedIndex!=-1) {
                selectedId=itemsSentencesFiltered[listBoxSentence.SelectedIndex];
            }
            
            SentenceRefreshFilteredList();

            listBoxSentence.Items.Clear();
            for (int i=0; i<itemsSentencesFiltered.Count; i++) {
                ItemSentence item = itemsSentencesFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSentence.Items.Add(textToAdd);
            }

            //SetListBoxSentence();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsSentencesFiltered.Count; i++){
                    if (itemsSentencesFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxSentence.SelectedIndex=-1;
                    CurrentSentence=null;
                } else listBoxSentence.SelectedIndex=outIndex;
            } else {
                listBoxSentence.SelectedIndex=-1;
                CurrentSentence=null;
            }
            SetCurrentSentence();
        }
            
        void RemoveCurrentSentence(object sender, EventArgs e) {
            itemsSentences.Remove(CurrentSentence);
        }

        void SetListBoxSentence() { 
            string filter=textBoxSentenceFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxSentence.SelectedIndex;
            listBoxSentence.Items.Clear();
            for (int i=0; i<itemsSentencesFiltered.Count; i++) {
                ItemSentence item = itemsSentencesFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSentence.Items.Add(textToAdd);
            }

            if (index>=listBoxSentence.Items.Count)index=listBoxSentence.Items.Count-1;
            listBoxSentence.SelectedIndex=index;
        }

        void SentenceRefreshFilteredList() {
            if (itemsSentencesFiltered==null) itemsSentencesFiltered=new List<ItemSentence>();
            itemsSentencesFiltered.Clear();
            string filter=textBoxSentenceFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsSentences.Count; i++) {
                    ItemSentence item = itemsSentences[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsSentencesFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsSentences.Count; i++) {
                    ItemSentence item = itemsSentences[i];
                    itemsSentencesFiltered.Add(item);
                }
            }
        }
               
        void AddNewItemSentence() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentSentence();

            var newItem=new ItemSentence();
          //  newItem.ID=itemsSentences.Count;
            itemsSentences.Add(newItem);
            CurrentSentence=newItem;
            SentenceRefreshFilteredList();
            SetListBoxSentence(); 
            ListBoxSetCurrentSentence();
            SetCurrentSentence(); 
            
            doingJob=false;
        }
   
        void RemoveItemSentence(ItemSentence item) { 
            Edited=true;
            ChangeCaptionText();
            itemsSentences.Remove(item);
            SentenceRefreshFilteredList();
            SetListBoxSentence();
            SetCurrentSentence();
        } 
           
        void SetCurrentSentence(){
            if (itemsSentencesFiltered.Count==0) {
                SetNoneSentence();
                return;
            }

            int index=listBoxSentence.SelectedIndex;
            if (index>=itemsSentencesFiltered.Count) index=itemsSentencesFiltered.Count-1;
            if (index<0) index=0;
            CurrentSentence=itemsSentencesFiltered[index];
            
            textBoxSentenceSource.Visible=true;
            textBoxSentenceTo.Visible=true;
            labelSentenceSource.Visible=true;
            labelSentenceTo.Visible=true; 

           textBoxSentenceSource.Text= CurrentSentence.From;
           textBoxSentenceTo.Text= CurrentSentence.To;
      
        }
         
        void ListBoxSetCurrentSentence() {
            for (int indexCur=0; indexCur<itemsSentencesFiltered.Count; indexCur++) {
                if (itemsSentences[indexCur]==CurrentSentence) { 
                    int indexList=listBoxSentence.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxSentence.SelectedIndex=indexCur;
                    break;
                }
            }
        }  

        //int GetListBoxSelectedIndexSentence() {
        //    if (listBoxSentences.SelectedIndex==-1) return -1;

        //    string filter=textBoxFilterSentence.Text;
        //    bool useFilter=filter=="" || filter=="*";

        //    if (useFilter) { 
        //        var item=itemsSentencesFiltered[listBoxSentences.SelectedIndex];
        //        return item.ID;
        //        //for (int i=0; i<itemsSentences.Count; i++){
        //        //    if (i==item) return i;
        //        //}
        //    } else { 
        //        return listBoxSentences.SelectedIndex;
        //    }

        //    return -1;
        //}
        
        void SaveCurrentSentence() {
            if (CurrentSentence==null) return;
                     
            CurrentSentence.From=textBoxSentenceSource.Text;
            CurrentSentence.To=textBoxSentenceTo.Text;
        } 
              
        void SetNoneSentence(){ 
            textBoxSentenceSource.Text="";
            textBoxSentenceTo.Text="";
            textBoxSentenceSource.Visible=false;
            textBoxSentenceTo.Visible=false;
            labelSentenceSource.Visible=false;
            labelSentenceTo.Visible=false;
        }
                
        void ClearSentences(){ 
            listBoxSentence.Items.Clear();
            SetNoneSentence();
            itemsSentencesFiltered?.Clear();
            itemsSentences?.Clear();
            CurrentSentence=null;
        }
        #endregion

        #region Phrase
        void ListBoxPhrases_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentPhrase();
            
            int index=listBoxPhrase.SelectedIndex;
            if (itemsPhrases.Count==0) {
                SetNonePhrase();
                return;
            }
            if (index>=itemsPhrases.Count) 
                index=itemsPhrases.Count-1;
            if (index<0)
                index=0;
           
            CurrentPhrase=itemsPhrases[index];
            SetCurrentPhrase();
            SetListBoxPhrase();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void ButtonPhraseAdd_Click(object sender, EventArgs e) {
            AddNewItemPhrase();
        }

        void ButtonPhraseRemove_Click(object sender, EventArgs e) {
            RemoveItemPhrase(CurrentPhrase);
            TextBoxPhraseFilter_TextChanged(null, new EventArgs());
        }
        
        void TextBoxPhraseFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentPhrase();

            // Získej aktuální prvek
            ItemPhrase selectedId=null;
            if (listBoxPhrase.SelectedIndex!=-1) {
                selectedId=itemsPhrasesFiltered[listBoxPhrase.SelectedIndex];
            }
            
            PhraseRefreshFilteredList();

            listBoxPhrase.Items.Clear();
            for (int i=0; i<itemsPhrasesFiltered.Count; i++) {
                ItemPhrase item = itemsPhrasesFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPhrase.Items.Add(textToAdd);
            }

            //SetListBoxPhrase();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsPhrasesFiltered.Count; i++){
                    if (itemsPhrasesFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxPhrase.SelectedIndex=-1;
                    CurrentPhrase=null;
                } else listBoxPhrase.SelectedIndex=outIndex;
            } else {
                listBoxPhrase.SelectedIndex=-1;
                CurrentPhrase=null;
            }
            SetCurrentPhrase();
        }
            
        void RemoveCurrentPhrase(object sender, EventArgs e) {
            itemsPhrases.Remove(CurrentPhrase);
        }

        void SetListBoxPhrase() { 
            string filter=textBoxPhraseFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxPhrase.SelectedIndex;
            listBoxPhrase.Items.Clear();
            for (int i=0; i<itemsPhrasesFiltered.Count; i++) {
                ItemPhrase item = itemsPhrasesFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPhrase.Items.Add(textToAdd);
            }

            if (index>=listBoxPhrase.Items.Count)index=listBoxPhrase.Items.Count-1;
            listBoxPhrase.SelectedIndex=index;
        }

        void PhraseRefreshFilteredList() {
            if (itemsPhrasesFiltered==null) itemsPhrasesFiltered=new List<ItemPhrase>();
            itemsPhrasesFiltered.Clear();
            string filter=textBoxPhraseFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsPhrases.Count; i++) {
                    ItemPhrase item = itemsPhrases[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsPhrasesFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsPhrases.Count; i++) {
                    ItemPhrase item = itemsPhrases[i];
                    itemsPhrasesFiltered.Add(item);
                }
            }
        }
               
        void AddNewItemPhrase() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentPhrase();

            var newItem=new ItemPhrase();
          //  newItem.ID=itemsPhrases.Count;
            itemsPhrases.Add(newItem);
            CurrentPhrase=newItem;
            PhraseRefreshFilteredList();
            SetListBoxPhrase(); 
            ListBoxSetCurrentPhrase();
            SetCurrentPhrase(); 
            
            doingJob=false;
        }
   
        void RemoveItemPhrase(ItemPhrase item) { 
            Edited=true;
            ChangeCaptionText();
            itemsPhrases.Remove(item);
            PhraseRefreshFilteredList();
            SetListBoxPhrase();
            SetCurrentPhrase();
        } 
           
        void SetCurrentPhrase(){
            if (itemsPhrasesFiltered.Count==0) {
                SetNonePhrase();
                return;
            }

            int index=listBoxPhrase.SelectedIndex;
            if (index>=itemsPhrasesFiltered.Count) index=itemsPhrasesFiltered.Count-1;
            if (index<0) index=0;
            CurrentPhrase=itemsPhrasesFiltered[index];
            
            textBoxPhraseFrom.Visible=true;
            textBoxPhraseTo.Visible=true;
            labelPhraseFrom.Visible=true;
            labelPhraseTo.Visible=true; 

           textBoxPhraseFrom.Text= CurrentPhrase.From;
           textBoxPhraseTo.Text= CurrentPhrase.To;
      
        }
         
        void ListBoxSetCurrentPhrase() {
            for (int indexCur=0; indexCur<itemsPhrasesFiltered.Count; indexCur++) {
                if (itemsPhrases[indexCur]==CurrentPhrase) { 
                    int indexList=listBoxPhrase.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxPhrase.SelectedIndex=indexCur;
                    break;
                }
            }
        }  

        //int GetListBoxSelectedIndexPhrase() {
        //    if (listBoxPhrases.SelectedIndex==-1) return -1;

        //    string filter=textBoxFilterPhrase.Text;
        //    bool useFilter=filter=="" || filter=="*";

        //    if (useFilter) { 
        //        var item=itemsPhrasesFiltered[listBoxPhrases.SelectedIndex];
        //        return item.ID;
        //        //for (int i=0; i<itemsPhrases.Count; i++){
        //        //    if (i==item) return i;
        //        //}
        //    } else { 
        //        return listBoxPhrases.SelectedIndex;
        //    }

        //    return -1;
        //}
        
        void SaveCurrentPhrase() {
            if (CurrentPhrase==null) return;
                     
            CurrentPhrase.From=textBoxPhraseFrom.Text;
            CurrentPhrase.To=textBoxPhraseTo.Text;
        } 
              
        void SetNonePhrase(){ 
            textBoxPhraseFrom.Text="";
            textBoxPhraseTo.Text="";
            textBoxPhraseFrom.Visible=false;
            textBoxPhraseTo.Visible=false;
            labelPhraseFrom.Visible=false;
            labelPhraseTo.Visible=false;
        }
                
        void ClearPhrase(){ 
            listBoxPhrase.Items.Clear();
            SetNonePhrase();
            itemsPhrasesFiltered?.Clear();
            itemsPhrases?.Clear();
            CurrentPhrase=null;
        }
        #endregion
 
        #region SimpleWord
        void ListBoxSimpleWord_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentSimpleWord();
            
            int index=listBoxSimpleWord.SelectedIndex;
            if (itemsSimpleWords.Count==0) {
                SetNoneSimpleWord();
                return;
            }
            if (index>=itemsSimpleWords.Count)    index=itemsSimpleWords.Count-1;
            if (index<0) index=0;
           
            CurrentSimpleWord=itemsSimpleWords[index];
            SetCurrentSimpleWord();
            SetListBoxSimpleWord();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void ButtonSimpleWordAdd_Click(object sender, EventArgs e) {
            AddNewItemSimpleWord();
        }

        void ButtonSimpleWordRemove_Click(object sender, EventArgs e) {
            RemoveItemSimpleWord(CurrentSimpleWord);
            TextBoxSimpleWordFilter_TextChanged(null, new EventArgs());
        }
        
        void TextBoxSimpleWordFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentSimpleWord();
            DrawingControl.SuspendDrawing(listBoxSimpleWord);
            // Získej aktuální prvek
            ItemSimpleWord selectedId=null;
            if (listBoxSimpleWord.SelectedIndex!=-1) {
                selectedId=itemsSimpleWordsFiltered[listBoxSimpleWord.SelectedIndex];
            }
            
            SimpleWordRefreshFilteredList();

            listBoxSimpleWord.Items.Clear();
            for (int i=0; i<itemsSimpleWordsFiltered.Count; i++) {
                ItemSimpleWord item = itemsSimpleWordsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSimpleWord.Items.Add(textToAdd);
            }

            //SetListBoxSimpleWord();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsSimpleWordsFiltered.Count; i++){
                    if (itemsSimpleWordsFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxSimpleWord.SelectedIndex=-1;
                    CurrentSimpleWord=null;
                } else listBoxSimpleWord.SelectedIndex=outIndex;
            } else {
                listBoxSimpleWord.SelectedIndex=-1;
                CurrentSimpleWord=null;
            }
            SetCurrentSimpleWord();
            DrawingControl.ResumeDrawing(listBoxSimpleWord);
        }
            
        void RemoveCurrentSimpleWord(object sender, EventArgs e) {
            itemsSimpleWords.Remove(CurrentSimpleWord);
        }

        void SetListBoxSimpleWord() { 
            DrawingControl.SuspendDrawing(listBoxSimpleWord);
            string filter=textBoxSimpleWordFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxSimpleWord.SelectedIndex;
            listBoxSimpleWord.Items.Clear();
            for (int i=0; i<itemsSimpleWordsFiltered.Count; i++) {
                ItemSimpleWord item = itemsSimpleWordsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSimpleWord.Items.Add(textToAdd);
            }

            if (index>=listBoxSimpleWord.Items.Count)index=listBoxSimpleWord.Items.Count-1;
            listBoxSimpleWord.SelectedIndex=index;
            
            DrawingControl.ResumeDrawing(listBoxSimpleWord);
        }

        void SimpleWordRefreshFilteredList() {
            if (itemsSimpleWordsFiltered==null) itemsSimpleWordsFiltered=new List<ItemSimpleWord>();
            itemsSimpleWordsFiltered.Clear();
            string filter=textBoxSimpleWordFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsSimpleWords.Count; i++) {
                    ItemSimpleWord item = itemsSimpleWords[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsSimpleWordsFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsSimpleWords.Count; i++) {
                    ItemSimpleWord item = itemsSimpleWords[i];
                    itemsSimpleWordsFiltered.Add(item);
                }
            }
        }
               
        void AddNewItemSimpleWord() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentSimpleWord();

            var newItem=new ItemSimpleWord();
            // newItem.ID=itemsSimpleWords.Count;
            itemsSimpleWords.Add(newItem);
            CurrentSimpleWord=newItem;
            SimpleWordRefreshFilteredList();
            SetListBoxSimpleWord(); 
            ListBoxSetCurrentSimpleWord();
            SetCurrentSimpleWord(); 
            
            doingJob=false;
        }
   
        void RemoveItemSimpleWord(ItemSimpleWord item) { 
            Edited=true;
            ChangeCaptionText();
            itemsSimpleWords.Remove(item);
            SimpleWordRefreshFilteredList();
            SetListBoxSimpleWord();
            SetCurrentSimpleWord();
        } 
           
        void SetCurrentSimpleWord(){
            if (itemsSimpleWordsFiltered.Count==0) {
                SetNoneSimpleWord();
                return;
            }

            int index=listBoxSimpleWord.SelectedIndex;
            if (index>=itemsSimpleWordsFiltered.Count) index=itemsSimpleWordsFiltered.Count-1;
            if (index<0) index=0;
            CurrentSimpleWord=itemsSimpleWordsFiltered[index];
            
            textBoxSimpleWordFrom.Visible=true;
            textBoxSimpleWordTo.Visible=true;
            labelSimpleWordFrom.Visible=true;
            labelSimpleWordTo.Visible=true; 

           textBoxSimpleWordFrom.Text= CurrentSimpleWord.From;
           textBoxSimpleWordTo.Text= CurrentSimpleWord.To;
      
        }
         
        void ListBoxSetCurrentSimpleWord() {
            DrawingControl.SuspendDrawing(listBoxSimpleWord);
            for (int indexCur=0; indexCur<itemsSimpleWordsFiltered.Count; indexCur++) {
                if (itemsSimpleWords[indexCur]==CurrentSimpleWord) { 
                    int indexList=listBoxSimpleWord.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxSimpleWord.SelectedIndex=indexCur;
                    break;
                }
            }
            DrawingControl.ResumeDrawing(listBoxSimpleWord);
        }  

        //int GetListBoxSelectedIndexSimpleWord() {
        //    if (listBoxSimpleWords.SelectedIndex==-1) return -1;

        //    string filter=textBoxFilterSimpleWord.Text;
        //    bool useFilter=filter=="" || filter=="*";

        //    if (useFilter) { 
        //        var item=itemsSimpleWordsFiltered[listBoxSimpleWords.SelectedIndex];
        //        return item.ID;
        //        //for (int i=0; i<itemsSimpleWords.Count; i++){
        //        //    if (i==item) return i;
        //        //}
        //    } else { 
        //        return listBoxSimpleWords.SelectedIndex;
        //    }

        //    return -1;
        //}
        
        void SaveCurrentSimpleWord() {
            if (CurrentSimpleWord==null) return;
                     
            CurrentSimpleWord.From=textBoxSimpleWordFrom.Text;
            CurrentSimpleWord.To=textBoxSimpleWordTo.Text;
        } 
              
        void SetNoneSimpleWord(){ 
            textBoxSimpleWordFrom.Text="";
            textBoxSimpleWordTo.Text="";
            textBoxSimpleWordFrom.Visible=false;
            textBoxSimpleWordTo.Visible=false;
            labelSimpleWordFrom.Visible=false;
            labelSimpleWordTo.Visible=false;
        }
                
        void ClearSimpleWord(){ 
            listBoxSimpleWord.Items.Clear();
            SetNoneSimpleWord();
            itemsSimpleWordsFiltered?.Clear();
            itemsSimpleWords?.Clear();
            CurrentSimpleWord=null;
        }
        #endregion
              
        #region SentencePattern  
        void TextBoxSentencePatternFilter_TextChanged(object sender, EventArgs e) {
              SaveCurrentSentencePattern();

            // Získej aktuální prvek
            ItemSentencePattern selectedId=null;
            if (listBoxSentencePatterns.SelectedIndex!=-1) {
                selectedId=itemsSentencePatternsFiltered[listBoxSentencePatterns.SelectedIndex];
            }
            
            SentencePatternRefreshFilteredList();

            listBoxSentencePatterns.Items.Clear();
            for (int i=0; i<itemsSentencePatternsFiltered.Count; i++) {
                ItemSentencePattern item = itemsSentencePatternsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSentencePatterns.Items.Add(textToAdd);
            }

            //SetListBoxSimpleWord();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsSentencePatternsFiltered.Count; i++){
                    if (itemsSentencePatternsFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxSentencePatterns.SelectedIndex=-1;
                    CurrentSentencePattern=null;
                } else listBoxSentencePatterns.SelectedIndex=outIndex;
            } else {
                listBoxSentencePatterns.SelectedIndex=-1;
                CurrentSentencePattern=null;
            }
            SetCurrentSentencePattern();
          //  SetListBoxSentencePattern();
        }
        
        void SentencePatternRefreshFilteredList() {
            if (itemsSentencePatternsFiltered==null) itemsSentencePatternsFiltered=new List<ItemSentencePattern>();
            itemsSentencePatternsFiltered.Clear();
            string filter=textBoxSentencePatternFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsSentencePatterns.Count; i++) {
                    ItemSentencePattern item = itemsSentencePatterns[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsSentencePatternsFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsSentencePatterns.Count; i++) {
                    ItemSentencePattern item = itemsSentencePatterns[i];
                    itemsSentencePatternsFiltered.Add(item);
                }
            }
        }
               
        void ButtonSentencePatternAdd_Click(object sender, EventArgs e) {
            AddNewItemSentencePattern();
        } 
        
        void ButtonSentencePatternRemove_Click(object sender, EventArgs e) {
            RemoveItemSentencePattern(CurrentSentencePattern);
            TextBoxSentencePatternFilter_TextChanged(null, new EventArgs());
        }

        //void SetListBoxSentencePattern() { 
        //    string filter=textBoxFilterSentencePattern.Text;
        //    bool useFilter = filter!="" && filter!="*"; 
           
        //    int index=listBoxSentencePatterns.SelectedIndex;
        //    listBoxSentencePatterns.Items.Clear();
        //    foreach (ItemSentencePattern item in itemsSentencePatterns) {
        //        if (useFilter) {
        //            if (!item.Filter(filter)) { 
        //                continue;    
        //            }
        //        }
        //        string textToAdd=item.GetText();
        //        if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

        //        listBoxSentencePatterns.Items.Add(textToAdd);
        //    }
        //    listBoxSentencePatterns.SelectedIndex=index;
        //}
        void SetListBoxSentencePattern() { 
            string filter=textBoxSentencePatternFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxSentencePatterns.SelectedIndex;
            listBoxSentencePatterns.Items.Clear();
            for (int i=0; i<itemsSentencePatternsFiltered.Count; i++) {
                ItemSentencePattern item = itemsSentencePatternsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSentencePatterns.Items.Add(textToAdd);
            }

            if (index>=listBoxSentencePatterns.Items.Count)index=listBoxSentencePatterns.Items.Count-1;
            listBoxSentencePatterns.SelectedIndex=index;
        }

        void AddNewItemSentencePattern() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentSentencePattern();

            var newItem=new ItemSentencePattern();
            itemsSentencePatterns.Add(newItem);
            CurrentSentencePattern=newItem;
           
            //SetListBoxSentencePattern(); 
            //ListBoxSetCurrentSentencePattern();
            //SetCurrentSentencePattern(); 

            SentencePatternRefreshFilteredList();
            SetListBoxSentencePattern(); 
            ListBoxSetCurrentSentencePattern();
            SetCurrentSentencePattern(); 

            doingJob=false;
        }
   
        void RemoveItemSentencePattern(ItemSentencePattern item) { 
            Edited=true;
            ChangeCaptionText();
            itemsSentencePatterns.Remove(item);
            SetListBoxSentencePattern();
            SetCurrentSentencePattern();
        } 
           
        void SetCurrentSentencePattern(){
            if (itemsSentencePatternsFiltered.Count==0) {
                SetNoneSentencePattern();
                return;
            }

            int index=listBoxSentencePatterns.SelectedIndex;
            if (index>itemsSentencePatternsFiltered.Count) index=itemsSentencePatternsFiltered.Count-1;
            if (index<0) index=0;
            CurrentSentencePattern=itemsSentencePatternsFiltered[index];
          //  throw new Exception();

            textBoxSentencePatternFrom.Text=CurrentSentencePattern.PatternSource;
            textBoxSentencePatternTo.Text=CurrentSentencePattern.PatternOutput;
            textBoxSentencePatternTo.Visible=true;
            textBoxSentencePatternFrom.Visible=true;
            labelSentencePatternInfo.Visible=true;
            labelSentencePatternFrom.Visible=true;
            labelSentencePatternTo.Visible=true;
        }
         
        void ListBoxSetCurrentSentencePattern() {
            for (int indexCur=0; indexCur<itemsSentencePatternsFiltered.Count; indexCur++) {
                if (itemsSentencePatterns[indexCur]==CurrentSentencePattern) { 
                    int indexList=listBoxSentencePatterns.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxSentencePatterns.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void SaveCurrentSentencePattern() {
            if (CurrentSentencePattern==null) return;
          
            CurrentSentencePattern.PatternSource=textBoxSentencePatternFrom.Text;
            CurrentSentencePattern.PatternOutput=textBoxSentencePatternTo.Text;
        } 
          
        void RemoveCurrentSentencePattern(object sender, EventArgs e) {
            itemsSentencePatterns.Remove(CurrentSentencePattern);
        }

        void ListBoxSentencePattern_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentSentencePattern();
            
            int index=listBoxSentencePatterns.SelectedIndex;
            if (itemsSentencePatterns.Count==0) {
                SetNoneSentencePattern();
                return;
            }
            if (index>=itemsSentencePatterns.Count)  index=itemsSentencePatterns.Count-1;
            if (index<0) index=0;
           
            CurrentSentencePattern=itemsSentencePatterns[index];
            SetCurrentSentencePattern();
            SetListBoxSentencePattern();
          //  SetCurrent();
            doingJob=false;
        }
        
        void SetNoneSentencePattern() { 
            textBoxSentencePatternFrom.Text="";
            textBoxSentencePatternTo.Text="";
            textBoxSentencePatternTo.Visible=false;
            textBoxSentencePatternFrom.Visible=false;
            labelSentencePatternInfo.Visible=false;
            labelSentencePatternFrom.Visible=false;
            labelSentencePatternTo.Visible=false;
        }

        void ClearSentencePattern(){ 
            listBoxSentencePatterns.Items.Clear();
            SetNoneSentencePattern();
            itemsSentencePatternsFiltered?.Clear();
            itemsSentencePatterns?.Clear();
            CurrentSentencePattern=null;
        }
        #endregion

        #region SentencePatternPart  
        void TextBoxSentencePatternPartFilter_TextChanged(object sender, EventArgs e) {
              SaveCurrentSentencePatternPart();

            // Získej aktuální prvek
            ItemSentencePatternPart selectedId=null;
            if (listBoxSentencePatternPart.SelectedIndex!=-1) {
                selectedId=itemsSentencePatternPartsFiltered[listBoxSentencePatternPart.SelectedIndex];
            }
            
            SentencePatternPartRefreshFilteredList();

            listBoxSentencePatternPart.Items.Clear();
            for (int i=0; i<itemsSentencePatternPartsFiltered.Count; i++) {
                ItemSentencePatternPart item = itemsSentencePatternPartsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSentencePatternPart.Items.Add(textToAdd);
            }

            //SetListBoxSimpleWord();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsSentencePatternPartsFiltered.Count; i++){
                    if (itemsSentencePatternPartsFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxSentencePatternPart.SelectedIndex=-1;
                    CurrentSentencePatternPart=null;
                } else listBoxSentencePatternPart.SelectedIndex=outIndex;
            } else {
                listBoxSentencePatternPart.SelectedIndex=-1;
                CurrentSentencePatternPart=null;
            }
            SetCurrentSentencePatternPart();
          //  SetListBoxSentencePatternPart();
        }
        
        void SentencePatternPartRefreshFilteredList() {
            if (itemsSentencePatternPartsFiltered==null) itemsSentencePatternPartsFiltered=new List<ItemSentencePatternPart>();
            itemsSentencePatternPartsFiltered.Clear();
            string filter=textBoxSentencePatternPartFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsSentencePatternParts.Count; i++) {
                    ItemSentencePatternPart item = itemsSentencePatternParts[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsSentencePatternPartsFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsSentencePatternParts.Count; i++) {
                    ItemSentencePatternPart item = itemsSentencePatternParts[i];
                    itemsSentencePatternPartsFiltered.Add(item);
                }
            }
        }
               
        void ButtonSentencePatternPartAdd_Click(object sender, EventArgs e) {
            AddNewItemSentencePatternPart();
        } 
        
        void ButtonSentencePatternPartRemove_Click(object sender, EventArgs e) {
            RemoveItemSentencePatternPart(CurrentSentencePatternPart);
            TextBoxSentencePatternPartFilter_TextChanged(null, new EventArgs());
        }

        //void SetListBoxSentencePatternPart() { 
        //    string filter=textBoxFilterSentencePatternPart.Text;
        //    bool useFilter = filter!="" && filter!="*"; 
           
        //    int index=listBoxSentencePatternParts.SelectedIndex;
        //    listBoxSentencePatternParts.Items.Clear();
        //    foreach (ItemSentencePatternPart item in itemsSentencePatternParts) {
        //        if (useFilter) {
        //            if (!item.Filter(filter)) { 
        //                continue;    
        //            }
        //        }
        //        string textToAdd=item.GetText();
        //        if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

        //        listBoxSentencePatternParts.Items.Add(textToAdd);
        //    }
        //    listBoxSentencePatternParts.SelectedIndex=index;
        //}
        void SetListBoxSentencePatternPart() { 
            string filter=textBoxSentencePatternPartFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxSentencePatternPart.SelectedIndex;
            listBoxSentencePatternPart.Items.Clear();
            for (int i=0; i<itemsSentencePatternPartsFiltered.Count; i++) {
                ItemSentencePatternPart item = itemsSentencePatternPartsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSentencePatternPart.Items.Add(textToAdd);
            }

            if (index>=listBoxSentencePatternPart.Items.Count)index=listBoxSentencePatternPart.Items.Count-1;
            listBoxSentencePatternPart.SelectedIndex=index;
        }

        void AddNewItemSentencePatternPart() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentSentencePatternPart();

            var newItem=new ItemSentencePatternPart();
            itemsSentencePatternParts.Add(newItem);
            CurrentSentencePatternPart=newItem;
           
            //SetListBoxSentencePatternPart(); 
            //ListBoxSetCurrentSentencePatternPart();
            //SetCurrentSentencePatternPart(); 

            SentencePatternPartRefreshFilteredList();
            SetListBoxSentencePatternPart(); 
            ListBoxSetCurrentSentencePatternPart();
            SetCurrentSentencePatternPart(); 

            doingJob=false;
        }
   
        void RemoveItemSentencePatternPart(ItemSentencePatternPart item) { 
            Edited=true;
            ChangeCaptionText();
            itemsSentencePatternParts.Remove(item);
            SetListBoxSentencePatternPart();
            SetCurrentSentencePatternPart();
        } 
           
        void SetCurrentSentencePatternPart(){
            if (itemsSentencePatternPartsFiltered.Count==0) {
                SetNoneSentencePatternPart();
                return;
            }

            int index=listBoxSentencePatternPart.SelectedIndex;
            if (index>itemsSentencePatternPartsFiltered.Count) index=itemsSentencePatternPartsFiltered.Count-1;
            if (index<0) index=0;
            CurrentSentencePatternPart=itemsSentencePatternPartsFiltered[index];
          //  throw new Exception();

            textBoxSentencePatternPartFrom.Text=CurrentSentencePatternPart.PatternSource;
            textBoxSentencePatternPartTo.Text=CurrentSentencePatternPart.PatternOutput;
            textBoxSentencePatternPartTo.Visible=true;
            textBoxSentencePatternPartFrom.Visible=true;
            //labelSentencePatternPartInfo.Visible=true;
            labelSentencePatternPartFrom.Visible=true;
            labelSentencePatternPartTo.Visible=true;
        }
         
        void ListBoxSetCurrentSentencePatternPart() {
            for (int indexCur=0; indexCur<itemsSentencePatternPartsFiltered.Count; indexCur++) {
                if (itemsSentencePatternParts[indexCur]==CurrentSentencePatternPart) { 
                    int indexList=listBoxSentencePatternPart.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxSentencePatternPart.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void SaveCurrentSentencePatternPart() {
            if (CurrentSentencePatternPart==null) return;
          
            CurrentSentencePatternPart.PatternSource=textBoxSentencePatternPartFrom.Text;
            CurrentSentencePatternPart.PatternOutput=textBoxSentencePatternPartTo.Text;
        } 
          
        void RemoveCurrentSentencePatternPart(object sender, EventArgs e) {
            itemsSentencePatternParts.Remove(CurrentSentencePatternPart);
        }

        void ListBoxSentencePatternPart_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentSentencePatternPart();
            
            int index=listBoxSentencePatternPart.SelectedIndex;
            if (itemsSentencePatternParts.Count==0) {
                SetNoneSentencePatternPart();
                return;
            }
            if (index>=itemsSentencePatternParts.Count)  index=itemsSentencePatternParts.Count-1;
            if (index<0) index=0;
           
            CurrentSentencePatternPart=itemsSentencePatternParts[index];
            SetCurrentSentencePatternPart();
            SetListBoxSentencePatternPart();
          //  SetCurrent();
            doingJob=false;
        }
        
        void SetNoneSentencePatternPart() { 
            textBoxSentencePatternPartFrom.Text="";
            textBoxSentencePatternPartTo.Text="";
            textBoxSentencePatternPartTo.Visible=false;
            textBoxSentencePatternPartFrom.Visible=false;
            //labelSentencePatternPartInfo.Visible=false;
            labelSentencePatternPartFrom.Visible=false;
            labelSentencePatternPartTo.Visible=false;
        }

        void ClearSentencePatternPart(){ 
            listBoxSentencePatternPart.Items.Clear();
            SetNoneSentencePatternPart();
            itemsSentencePatternPartsFiltered?.Clear();
            itemsSentencePatternParts?.Clear();
            CurrentSentencePatternPart=null;
        }
        #endregion
  
        #region SentencePart  
        void TextBoxSentencePartFilter_TextChanged(object sender, EventArgs e) {
              SaveCurrentSentencePart();

            // Získej aktuální prvek
            ItemSentencePart selectedId=null;
            if (listBoxSentencePart.SelectedIndex!=-1) {
                selectedId=itemsSentencePartsFiltered[listBoxSentencePart.SelectedIndex];
            }
            
            SentencePartRefreshFilteredList();

            listBoxSentencePart.Items.Clear();
            for (int i=0; i<itemsSentencePartsFiltered.Count; i++) {
                ItemSentencePart item = itemsSentencePartsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSentencePart.Items.Add(textToAdd);
            }

            //SetListBoxSimpleWord();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsSentencePartsFiltered.Count; i++){
                    if (itemsSentencePartsFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxSentencePart.SelectedIndex=-1;
                    CurrentSentencePart=null;
                } else listBoxSentencePart.SelectedIndex=outIndex;
            } else {
                listBoxSentencePart.SelectedIndex=-1;
                CurrentSentencePart=null;
            }
            SetCurrentSentencePart();
          //  SetListBoxSentencePart();
        }
        
        void SentencePartRefreshFilteredList() {
            if (itemsSentencePartsFiltered==null) itemsSentencePartsFiltered=new List<ItemSentencePart>();
            itemsSentencePartsFiltered.Clear();
            string filter=textBoxSentencePartFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsSentenceParts.Count; i++) {
                    ItemSentencePart item = itemsSentenceParts[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsSentencePartsFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsSentenceParts.Count; i++) {
                    ItemSentencePart item = itemsSentenceParts[i];
                    itemsSentencePartsFiltered.Add(item);
                }
            }
        }
               
        void ButtonSentencePartAdd_Click(object sender, EventArgs e) {
            AddNewItemSentencePart();
        } 
        
        void ButtonSentencePartRemove_Click(object sender, EventArgs e) {
            RemoveItemSentencePart(CurrentSentencePart);
            TextBoxSentencePartFilter_TextChanged(null, new EventArgs());
        }

        //void SetListBoxSentencePart() { 
        //    string filter=textBoxFilterSentencePart.Text;
        //    bool useFilter = filter!="" && filter!="*"; 
           
        //    int index=listBoxSentenceParts.SelectedIndex;
        //    listBoxSentenceParts.Items.Clear();
        //    foreach (ItemSentencePart item in itemsSentenceParts) {
        //        if (useFilter) {
        //            if (!item.Filter(filter)) { 
        //                continue;    
        //            }
        //        }
        //        string textToAdd=item.GetText();
        //        if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

        //        listBoxSentenceParts.Items.Add(textToAdd);
        //    }
        //    listBoxSentenceParts.SelectedIndex=index;
        //}
        void SetListBoxSentencePart() { 
            string filter=textBoxSentencePartFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxSentencePart.SelectedIndex;
            listBoxSentencePart.Items.Clear();
            for (int i=0; i<itemsSentencePartsFiltered.Count; i++) {
                ItemSentencePart item = itemsSentencePartsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxSentencePart.Items.Add(textToAdd);
            }

            if (index>=listBoxSentencePart.Items.Count)index=listBoxSentencePart.Items.Count-1;
            listBoxSentencePart.SelectedIndex=index;
        }

        void AddNewItemSentencePart() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentSentencePart();

            var newItem=new ItemSentencePart();
            itemsSentenceParts.Add(newItem);
            CurrentSentencePart=newItem;
           
            //SetListBoxSentencePart(); 
            //ListBoxSetCurrentSentencePart();
            //SetCurrentSentencePart(); 

            SentencePartRefreshFilteredList();
            SetListBoxSentencePart(); 
            ListBoxSetCurrentSentencePart();
            SetCurrentSentencePart(); 

            doingJob=false;
        }
   
        void RemoveItemSentencePart(ItemSentencePart item) { 
            Edited=true;
            ChangeCaptionText();
            itemsSentenceParts.Remove(item);
            SetListBoxSentencePart();
            SetCurrentSentencePart();
        } 
           
        void SetCurrentSentencePart(){
            if (itemsSentencePartsFiltered.Count==0) {
                SetNoneSentencePart();
                return;
            }

            int index=listBoxSentencePart.SelectedIndex;
            if (index>itemsSentencePartsFiltered.Count) index=itemsSentencePartsFiltered.Count-1;
            if (index<0) index=0;
            CurrentSentencePart=itemsSentencePartsFiltered[index];
          //  throw new Exception();

            textBoxSentencePartFrom.Text=CurrentSentencePart.From;
            textBoxSentencePartTo.Text=CurrentSentencePart.To;
            textBoxSentencePartTo.Visible=true;
            textBoxSentencePartFrom.Visible=true;
            //labelSentencePartInfo.Visible=true;
            labelSentencePartFrom.Visible=true;
            labelSentencePartTo.Visible=true;
        }
         
        void ListBoxSetCurrentSentencePart() {
            for (int indexCur=0; indexCur<itemsSentencePartsFiltered.Count; indexCur++) {
                if (itemsSentenceParts[indexCur]==CurrentSentencePart) { 
                    int indexList=listBoxSentencePart.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxSentencePart.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void SaveCurrentSentencePart() {
            if (CurrentSentencePart==null) return;
          
            CurrentSentencePart.From=textBoxSentencePartFrom.Text;
            CurrentSentencePart.To=textBoxSentencePartTo.Text;
        } 
          
        void RemoveCurrentSentencePart(object sender, EventArgs e) {
            itemsSentenceParts.Remove(CurrentSentencePart);
        }

        void ListBoxSentencePart_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentSentencePart();
            
            int index=listBoxSentencePart.SelectedIndex;
            if (itemsSentenceParts.Count==0) {
                SetNoneSentencePart();
                return;
            }
            if (index>=itemsSentenceParts.Count)  index=itemsSentenceParts.Count-1;
            if (index<0) index=0;
           
            CurrentSentencePart=itemsSentenceParts[index];
            SetCurrentSentencePart();
            SetListBoxSentencePart();
          //  SetCurrent();
            doingJob=false;
        }
        
        void SetNoneSentencePart() { 
            textBoxSentencePartFrom.Text="";
            textBoxSentencePartTo.Text="";
            textBoxSentencePartTo.Visible=false;
            textBoxSentencePartFrom.Visible=false;
            //labelSentencePartInfo.Visible=false;
            labelSentencePartFrom.Visible=false;
            labelSentencePartTo.Visible=false;
        }

        void ClearSentencePart(){ 
            listBoxSentencePart.Items.Clear();
            SetNoneSentencePart();
            itemsSentencePartsFiltered?.Clear();
            itemsSentenceParts?.Clear();
            CurrentSentencePart=null;
        }
        #endregion

        #region NounPatternFrom
        void ListBoxPatternNounFrom_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternNounFromSaveCurrent();
            
            int index=listBoxPatternNounFrom.SelectedIndex;
            if (itemsPatternNounFrom.Count==0) {
                PatternNounFromSetNone();
                return;
            }
            if (index>=itemsPatternNounFrom.Count) 
                index=itemsPatternNounFrom.Count-1;
            if (index<0)
                index=0;
           
            CurrentPatternNounFrom=itemsPatternNounFrom[index];
            PatternNounFromSetCurrent();
            PatternNounFromSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void ButtonPatternNounFromAdd_Click(object sender, EventArgs e) {
            PatternNounFromAddNewItem();
        }

        void ButtonPatternNounFromRemove_Click(object sender, EventArgs e) {
            PatternNounFromRemoveItem(CurrentPatternNounFrom);
            TextBoxPatternNounFromFilter_TextChanged(null, new EventArgs());
        }
        
        void TextBoxPatternNounFromFilter_TextChanged(object sender, EventArgs e) {
            PatternNounFromSaveCurrent();

            // Získej aktuální prvek
            ItemPatternNoun selectedId=null;
            if (listBoxPatternNounFrom.SelectedIndex!=-1) {
                selectedId=itemsPatternNounFromFiltered[listBoxPatternNounFrom.SelectedIndex];
            }
            
            PatternNounFromRefreshFilteredList();

            listBoxPatternNounFrom.Items.Clear();
            for (int i=0; i<itemsPatternNounFromFiltered.Count; i++) {
                ItemPatternNoun item = itemsPatternNounFromFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternNounFrom.Items.Add(textToAdd);
            }

            //SetListBoxNoun();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsPatternNounFromFiltered.Count; i++){
                    if (itemsPatternNounFromFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1) {
                    listBoxPatternNounFrom.SelectedIndex=-1;
                    CurrentPatternNounFrom=null;
                } else listBoxPatternNounFrom.SelectedIndex=outIndex;
            } else {
                listBoxPatternNounFrom.SelectedIndex=-1;
                CurrentPatternNounFrom=null;
            }
            PatternNounFromSetCurrent();
        }
            
        void PatternNounFromRemoveCurrent(object sender, EventArgs e) {
            itemsPatternNounFrom.Remove(CurrentPatternNounFrom);
        }

        void PatternNounFromSetListBox() { 
            string filter=textBoxPatternNounFromFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxPatternNounFrom.SelectedIndex;
            listBoxPatternNounFrom.Items.Clear();
            for (int i=0; i<itemsPatternNounFromFiltered.Count; i++) {
                ItemPatternNoun item = itemsPatternNounFromFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternNounFrom.Items.Add(textToAdd);
            }

            if (index>=listBoxPatternNounFrom.Items.Count)index=listBoxPatternNounFrom.Items.Count-1;
            listBoxPatternNounFrom.SelectedIndex=index;
        }

        void PatternNounFromRefreshFilteredList() {
            if (itemsPatternNounFromFiltered==null) itemsPatternNounFromFiltered=new List<ItemPatternNoun>();
            itemsPatternNounFromFiltered.Clear();
            string filter=textBoxPatternNounFromFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsPatternNounFrom.Count; i++) {
                    ItemPatternNoun item = itemsPatternNounFrom[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsPatternNounFromFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsPatternNounFrom.Count; i++) {
                    ItemPatternNoun item = itemsPatternNounFrom[i];
                    itemsPatternNounFromFiltered.Add(item);
                }
            }
        }
               
        void PatternNounFromAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternNounFromSaveCurrent();

            var newItem=new ItemPatternNoun();
           // newItem.ID=itemsNouns.Count;
            itemsPatternNounFrom.Add(newItem);
            CurrentPatternNounFrom=newItem;
            PatternNounFromRefreshFilteredList();
            PatternNounFromSetListBox(); 
            PatternNounFromListBoxSetCurrent();
            PatternNounFromSetCurrent(); 
            
            doingJob=false;
        }
   
        void PatternNounFromRemoveItem(ItemPatternNoun item) { 
            Edited=true;
            ChangeCaptionText();
            itemsPatternNounFrom.Remove(item);
            PatternNounFromRefreshFilteredList();
            PatternNounFromSetListBox();
            PatternNounFromSetCurrent();
        } 
           
        void PatternNounFromSetCurrent(){
            if (itemsPatternNounFromFiltered.Count==0) {
                PatternNounFromSetNone();
                return;
            }

            int index=listBoxPatternNounFrom.SelectedIndex;
            if (index>=itemsPatternNounFromFiltered.Count) index=itemsPatternNounFromFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternNounFrom=itemsPatternNounFromFiltered[index];
            textBoxPatternNounFromName.Text=CurrentPatternNounFrom.Name;
            comboBoxPatternNounFromGender.SelectedIndex=(int)CurrentPatternNounFrom.Gender;

            textBoxPatternNounFromJ1.Text=CurrentPatternNounFrom.Shapes[0];
            textBoxPatternNounFromJ2.Text=CurrentPatternNounFrom.Shapes[1];
            textBoxPatternNounFromJ3.Text=CurrentPatternNounFrom.Shapes[2];
            textBoxPatternNounFromJ4.Text=CurrentPatternNounFrom.Shapes[3];
            textBoxPatternNounFromJ5.Text=CurrentPatternNounFrom.Shapes[4];
            textBoxPatternNounFromJ6.Text=CurrentPatternNounFrom.Shapes[5];
            textBoxPatternNounFromJ7.Text=CurrentPatternNounFrom.Shapes[6];

            textBoxPatternNounFromM1.Text=CurrentPatternNounFrom.Shapes[7];
            textBoxPatternNounFromM2.Text=CurrentPatternNounFrom.Shapes[8];
            textBoxPatternNounFromM3.Text=CurrentPatternNounFrom.Shapes[9];
            textBoxPatternNounFromM4.Text=CurrentPatternNounFrom.Shapes[10];
            textBoxPatternNounFromM5.Text=CurrentPatternNounFrom.Shapes[11];
            textBoxPatternNounFromM6.Text=CurrentPatternNounFrom.Shapes[12];
            textBoxPatternNounFromM7.Text=CurrentPatternNounFrom.Shapes[13];

            labelPatternNounFromFall.Visible=true;
            labelPatternNounFromInfo.Visible=true;
            labelPatternNounFromMultiple.Visible=true;
            labelPatternNounFromGender.Visible=true;

            labelPatternNounFromSingle.Visible=true;
            labelPatternNounFromName.Visible=true;
            tableLayoutPanelPatternNounFrom.Visible=true;

            textBoxPatternNounFromName.Visible=true;
            comboBoxPatternNounFromGender.Visible=true;
        }
         
        void PatternNounFromListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternNounFromFiltered.Count; indexCur++) {
                if (itemsPatternNounFrom[indexCur]==CurrentPatternNounFrom) { 
                    int indexList=listBoxPatternNounFrom.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxPatternNounFrom.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void PatternNounFromSaveCurrent() {
            if (CurrentPatternNounFrom==null) return;
                     
            CurrentPatternNounFrom.Name=textBoxPatternNounFromName.Text;
            CurrentPatternNounFrom.Gender=(GenderNoun)comboBoxPatternNounFromGender.SelectedIndex;

            CurrentPatternNounFrom.Shapes[0]=textBoxPatternNounFromJ1.Text;
            CurrentPatternNounFrom.Shapes[1]=textBoxPatternNounFromJ2.Text;
            CurrentPatternNounFrom.Shapes[2]=textBoxPatternNounFromJ3.Text;
            CurrentPatternNounFrom.Shapes[3]=textBoxPatternNounFromJ4.Text;
            CurrentPatternNounFrom.Shapes[4]=textBoxPatternNounFromJ5.Text;
            CurrentPatternNounFrom.Shapes[5]=textBoxPatternNounFromJ6.Text;
            CurrentPatternNounFrom.Shapes[6]=textBoxPatternNounFromJ7.Text;

            CurrentPatternNounFrom.Shapes[7]=textBoxPatternNounFromM1.Text;
            CurrentPatternNounFrom.Shapes[8]=textBoxPatternNounFromM2.Text;
            CurrentPatternNounFrom.Shapes[9]=textBoxPatternNounFromM3.Text;
            CurrentPatternNounFrom.Shapes[10]=textBoxPatternNounFromM4.Text;
            CurrentPatternNounFrom.Shapes[11]=textBoxPatternNounFromM5.Text;
            CurrentPatternNounFrom.Shapes[12]=textBoxPatternNounFromM6.Text;
            CurrentPatternNounFrom.Shapes[13]=textBoxPatternNounFromM7.Text;
        } 
              
        void PatternNounFromSetNone(){ 
            textBoxPatternNounFromName.Text="";
            comboBoxPatternNounFromGender.SelectedIndex=0;

            textBoxPatternNounFromJ1.Text="";
            textBoxPatternNounFromJ2.Text="";
            textBoxPatternNounFromJ3.Text="";
            textBoxPatternNounFromJ4.Text="";
            textBoxPatternNounFromJ5.Text="";
            textBoxPatternNounFromJ6.Text="";
            textBoxPatternNounFromJ7.Text="";

            textBoxPatternNounFromM1.Text="";
            textBoxPatternNounFromM2.Text="";
            textBoxPatternNounFromM3.Text="";
            textBoxPatternNounFromM4.Text="";
            textBoxPatternNounFromM5.Text="";
            textBoxPatternNounFromM6.Text="";
            textBoxPatternNounFromM7.Text="";
            textBoxPatternNounFromName.Visible=false;
            comboBoxPatternNounFromGender.Visible=false;
            labelPatternNounFromFall.Visible=false;
            labelPatternNounFromInfo.Visible=false;
            labelPatternNounFromMultiple.Visible=false;
            labelPatternNounFromGender.Visible=false;

            labelPatternNounFromSingle.Visible=false;
            labelPatternNounFromName.Visible=false;
            tableLayoutPanelPatternNounFrom.Visible=false;
        }
        
        void PatternNounFromClear() { 
            listBoxPatternNounFrom.Items.Clear();
            PatternNounFromSetNone();
            itemsPatternNounFromFiltered?.Clear();
            itemsPatternNounFrom?.Clear();
            CurrentPatternNounFrom=null;
        }
        #endregion
       
        #region NounPatternTo
        void ListBoxPatternNounTo_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternNounToSaveCurrent();
            
            int index=listBoxPatternNounTo.SelectedIndex;
            if (itemsPatternNounTo.Count==0) {
                PatternNounToSetNone();
                return;
            }
            if (index>=itemsPatternNounTo.Count) 
                index=itemsPatternNounTo.Count-1;
            if (index<0)
                index=0;
           
            CurrentPatternNounTo=itemsPatternNounTo[index];
            PatternNounToSetCurrent();
            PatternNounToSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void ButtonPatternNounToAdd_Click(object sender, EventArgs e) {
            PatternNounToAddNewItem();
        }

        void ButtonPatternNounToRemove_Click(object sender, EventArgs e) {
            PatternNounToRemoveItem(CurrentPatternNounTo);
            TextBoxPatternNounToFilter_TextChanged(null, new EventArgs());
        }
        
        void TextBoxPatternNounToFilter_TextChanged(object sender, EventArgs e) {
            PatternNounToSaveCurrent();

            // Získej aktuální prvek
            ItemPatternNoun selectedId=null;
            if (listBoxPatternNounTo.SelectedIndex!=-1) {
                selectedId=itemsPatternNounToFiltered[listBoxPatternNounTo.SelectedIndex];
            }
            
            PatternNounToRefreshFilteredList();

            listBoxPatternNounTo.Items.Clear();
            for (int i=0; i<itemsPatternNounToFiltered.Count; i++) {
                ItemPatternNoun item = itemsPatternNounToFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternNounTo.Items.Add(textToAdd);
            }

            //SetListBoxNoun();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsPatternNounToFiltered.Count; i++){
                    if (itemsPatternNounToFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1) {
                    listBoxPatternNounTo.SelectedIndex=-1;
                    CurrentPatternNounTo=null;
                } else listBoxPatternNounTo.SelectedIndex=outIndex;
            } else {
                listBoxPatternNounTo.SelectedIndex=-1;
                CurrentPatternNounTo=null;
            }
            PatternNounToSetCurrent();
        }
            
        void PatternNounToRemoveCurrent(object sender, EventArgs e) {
            itemsPatternNounTo.Remove(CurrentPatternNounTo);
        }

        void PatternNounToSetListBox() { 
            string filter=textBoxPatternNounToFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxPatternNounTo.SelectedIndex;
            listBoxPatternNounTo.Items.Clear();
            for (int i=0; i<itemsPatternNounToFiltered.Count; i++) {
                ItemPatternNoun item = itemsPatternNounToFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternNounTo.Items.Add(textToAdd);
            }

            if (index>=listBoxPatternNounTo.Items.Count)index=listBoxPatternNounTo.Items.Count-1;
            listBoxPatternNounTo.SelectedIndex=index;
        }

        void PatternNounToRefreshFilteredList() {
            if (itemsPatternNounToFiltered==null) itemsPatternNounToFiltered=new List<ItemPatternNoun>();
            itemsPatternNounToFiltered.Clear();
            string filter=textBoxPatternNounToFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsPatternNounTo.Count; i++) {
                    ItemPatternNoun item = itemsPatternNounTo[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsPatternNounToFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsPatternNounTo.Count; i++) {
                    ItemPatternNoun item = itemsPatternNounTo[i];
                    itemsPatternNounToFiltered.Add(item);
                }
            }
        }
               
        void PatternNounToAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternNounToSaveCurrent();

            var newItem=new ItemPatternNoun();
           // newItem.ID=itemsNouns.Count;
            itemsPatternNounTo.Add(newItem);
            CurrentPatternNounTo=newItem;
            PatternNounToRefreshFilteredList();
            PatternNounToSetListBox(); 
            PatternNounToListBoxSetCurrent();
            PatternNounToSetCurrent(); 
            
            doingJob=false;
        }
   
        void PatternNounToRemoveItem(ItemPatternNoun item) { 
            Edited=true;
            ChangeCaptionText();
            itemsPatternNounTo.Remove(item);
            PatternNounToRefreshFilteredList();
            PatternNounToSetListBox();
            PatternNounToSetCurrent();
        } 
           
        void PatternNounToSetCurrent(){
            if (itemsPatternNounToFiltered.Count==0) {
                PatternNounToSetNone();
                return;
            }

            int index=listBoxPatternNounTo.SelectedIndex;
            if (index>=itemsPatternNounToFiltered.Count) index=itemsPatternNounToFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternNounTo=itemsPatternNounToFiltered[index];
            textBoxPatternNounToName.Text=CurrentPatternNounTo.Name;
            comboBoxPatternNounToGender.SelectedIndex=(int)CurrentPatternNounTo.Gender;
            textBoxPatternNounToS1.Text=CurrentPatternNounTo.Shapes[0];
            textBoxPatternNounToS2.Text=CurrentPatternNounTo.Shapes[1];
            textBoxPatternNounToS3.Text=CurrentPatternNounTo.Shapes[2];
            textBoxPatternNounToS4.Text=CurrentPatternNounTo.Shapes[3];
            textBoxPatternNounToS5.Text=CurrentPatternNounTo.Shapes[4];
            textBoxPatternNounToS6.Text=CurrentPatternNounTo.Shapes[5];
            textBoxPatternNounToS7.Text=CurrentPatternNounTo.Shapes[6];

            textBoxPatternNounToM1.Text=CurrentPatternNounTo.Shapes[7];
            textBoxPatternNounToM2.Text=CurrentPatternNounTo.Shapes[8];
            textBoxPatternNounToM3.Text=CurrentPatternNounTo.Shapes[9];
            textBoxPatternNounToM4.Text=CurrentPatternNounTo.Shapes[10];
            textBoxPatternNounToM5.Text=CurrentPatternNounTo.Shapes[11];
            textBoxPatternNounToM6.Text=CurrentPatternNounTo.Shapes[12];
            textBoxPatternNounToM7.Text=CurrentPatternNounTo.Shapes[13];

            labelPatternNounToFall.Visible=true;
            labelPatternNounToInfo.Visible=true;
            labelPatternNounToMultiple.Visible=true;
            labelPatternNounToGender.Visible=true;

            labelPatternNounToSingle.Visible=true;
            labelPatternNounToName.Visible=true;
            tableLayoutPanelPatternNounTo.Visible=true;

            textBoxPatternNounToName.Visible=true;
            comboBoxPatternNounToGender.Visible=true;
        }
         
        void PatternNounToListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternNounToFiltered.Count; indexCur++) {
                if (itemsPatternNounTo[indexCur]==CurrentPatternNounTo) { 
                    int indexList=listBoxPatternNounTo.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxPatternNounTo.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void PatternNounToSaveCurrent() {
            if (CurrentPatternNounTo==null) return;
                     
            CurrentPatternNounTo.Name=textBoxPatternNounToName.Text;
            CurrentPatternNounTo.Gender=(GenderNoun)comboBoxPatternNounToGender.SelectedIndex;

            CurrentPatternNounTo.Shapes[0]=textBoxPatternNounToS1.Text;
            CurrentPatternNounTo.Shapes[1]=textBoxPatternNounToS2.Text;
            CurrentPatternNounTo.Shapes[2]=textBoxPatternNounToS3.Text;
            CurrentPatternNounTo.Shapes[3]=textBoxPatternNounToS4.Text;
            CurrentPatternNounTo.Shapes[4]=textBoxPatternNounToS5.Text;
            CurrentPatternNounTo.Shapes[5]=textBoxPatternNounToS6.Text;
            CurrentPatternNounTo.Shapes[6]=textBoxPatternNounToS7.Text;

            CurrentPatternNounTo.Shapes[ 7]=textBoxPatternNounToM1.Text;
            CurrentPatternNounTo.Shapes[ 8]=textBoxPatternNounToM2.Text;
            CurrentPatternNounTo.Shapes[ 9]=textBoxPatternNounToM3.Text;
            CurrentPatternNounTo.Shapes[10]=textBoxPatternNounToM4.Text;
            CurrentPatternNounTo.Shapes[11]=textBoxPatternNounToM5.Text;
            CurrentPatternNounTo.Shapes[12]=textBoxPatternNounToM6.Text;
            CurrentPatternNounTo.Shapes[13]=textBoxPatternNounToM7.Text;
        } 
              
        void PatternNounToSetNone(){ 
            textBoxPatternNounToName.Text="";
            comboBoxPatternNounToGender.SelectedIndex=0;

            textBoxPatternNounToS1.Text="";
            textBoxPatternNounToS2.Text="";
            textBoxPatternNounToS3.Text="";
            textBoxPatternNounToS4.Text="";
            textBoxPatternNounToS5.Text="";
            textBoxPatternNounToS6.Text="";
            textBoxPatternNounToS7.Text="";

            textBoxPatternNounToM1.Text="";
            textBoxPatternNounToM2.Text="";
            textBoxPatternNounToM3.Text="";
            textBoxPatternNounToM4.Text="";
            textBoxPatternNounToM5.Text="";
            textBoxPatternNounToM6.Text="";
            textBoxPatternNounToM7.Text="";
            textBoxPatternNounToName.Visible=false;
            comboBoxPatternNounToGender.Visible=false;
            labelPatternNounToFall.Visible=false;
            labelPatternNounToInfo.Visible=false;
            labelPatternNounToMultiple.Visible=false;
            labelPatternNounToGender.Visible=false;

            labelPatternNounToSingle.Visible=false;
            labelPatternNounToName.Visible=false;
            tableLayoutPanelPatternNounTo.Visible=false;
        }
        
        void PatternNounToClear() { 
            listBoxPatternNounTo.Items.Clear();
            PatternNounToSetNone();
            itemsPatternNounToFiltered?.Clear();
            itemsPatternNounTo?.Clear();
            CurrentPatternNounTo=null;
        }
        #endregion
     
        #region Noun
        void ListBoxNoun_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentNoun();
            
            int index=listBoxNoun.SelectedIndex;
            if (itemsNouns.Count==0) {
                SetNoneNoun();
                return;
            }
            if (index>=itemsNouns.Count) 
                index=itemsNouns.Count-1;
            if (index<0)
                index=0;
           
            CurrentNoun=itemsNouns[index];
            SetCurrentNoun();
            SetListBoxNoun();
            doingJob=false;
        }  
        
        void ButtonNounAdd_Click(object sender, EventArgs e) {
            AddNewItemNoun();
        }

        void ButtonNounRemove_Click(object sender, EventArgs e) {
            RemoveItemNoun(CurrentNoun);
            TextBoxNounFilter_TextChanged(null, new EventArgs());
        }
        
        void TextBoxNounFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentNoun();

            // Získej aktuální prvek
            ItemNoun selectedId=null;
            if (listBoxNoun.SelectedIndex!=-1) {
                selectedId=itemsNounsFiltered[listBoxNoun.SelectedIndex];
            }
            
            NounRefreshFilteredList();

            listBoxNoun.Items.Clear();
            for (int i=0; i<itemsNounsFiltered.Count; i++) {
                ItemNoun item = itemsNounsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxNoun.Items.Add(textToAdd);
            }

            //SetListBoxNoun();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsNounsFiltered.Count; i++){
                    if (itemsNounsFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxNoun.SelectedIndex=-1;
                    CurrentNoun=null;
                } else listBoxNoun.SelectedIndex=outIndex;
            } else {
                listBoxNoun.SelectedIndex=-1;
                CurrentNoun=null;
            }
            SetCurrentNoun();
        }
            
        void RemoveCurrentNoun(object sender, EventArgs e) {
            itemsNouns.Remove(CurrentNoun);
        }

        void SetListBoxNoun() { 
            string filter=textBoxNounFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxNoun.SelectedIndex;
            listBoxNoun.Items.Clear();
            for (int i=0; i<itemsNounsFiltered.Count; i++) {
                ItemNoun item = itemsNounsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxNoun.Items.Add(textToAdd);
            }

            if (index>=listBoxNoun.Items.Count)index=listBoxNoun.Items.Count-1;
            listBoxNoun.SelectedIndex=index;
        }

        void NounRefreshFilteredList() {
            if (itemsNounsFiltered==null) itemsNounsFiltered=new List<ItemNoun>();
            itemsNounsFiltered.Clear();
            string filter=textBoxNounFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsNouns.Count; i++) {
                    ItemNoun item = itemsNouns[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsNounsFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsNouns.Count; i++) {
                    ItemNoun item = itemsNouns[i];
                    itemsNounsFiltered.Add(item);
                }
            }
        }
               
        void AddNewItemNoun() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentNoun();

            var newItem=new ItemNoun();
           // newItem.ID=itemsNouns.Count;
            itemsNouns.Add(newItem);
            CurrentNoun=newItem;
            NounRefreshFilteredList();
            SetListBoxNoun(); 
            ListBoxSetCurrentNoun();
            SetCurrentNoun(); 
            
            doingJob=false;
        }
   
        void RemoveItemNoun(ItemNoun item) { 
            Edited=true;
            ChangeCaptionText();
            itemsNouns.Remove(item);
            NounRefreshFilteredList();
            SetListBoxNoun();
            SetCurrentNoun();
        } 
           
        void SetCurrentNoun(){
            if (itemsNounsFiltered.Count==0) {
                SetNoneNoun();
                return;
            }

            int index=listBoxNoun.SelectedIndex;
            if (index>=itemsNounsFiltered.Count) index=itemsNounsFiltered.Count-1;
            if (index<0) index=0;
            CurrentNoun=itemsNounsFiltered[index];
            
            textBoxNounFrom.Visible=true;
            textBoxNounTo.Visible=true;
            labelNounFrom.Visible=true;
            labelNounTo.Visible=true; 

            textBoxNounFrom.Text=CurrentNoun.From;
            textBoxNounTo.Text=CurrentNoun.To;

            if (CurrentNoun.PatternFrom=="") {
                comboBoxNounInputPatternFrom.SelectedIndex=-1;
                comboBoxNounInputPatternFrom.Text="";
            }
            else {
                comboBoxNounInputPatternFrom.Text=CurrentNoun.PatternFrom;
            }

            if (CurrentNoun.PatternTo=="")comboBoxNounInputPatternTo.SelectedIndex=-1;
            comboBoxNounInputPatternFrom.Text=CurrentNoun.PatternFrom;

            comboBoxNounInputPatternFrom.Items.Clear();
            comboBoxNounInputPatternTo.Items.Clear();
            foreach (ItemPatternNoun x in itemsPatternNounFrom) {
                comboBoxNounInputPatternFrom.Items.Add(x.Name);
            }
            foreach (ItemPatternNoun x in itemsPatternNounTo) {
                comboBoxNounInputPatternTo.Items.Add(x.Name);
            }
            
            comboBoxNounInputPatternTo.Text=CurrentNoun.PatternTo;
      
            comboBoxNounInputPatternFrom.Visible=true; 
            comboBoxNounInputPatternTo.Visible=true; 

            labelNounInputPatternFrom.Visible=true; 
            labelNounInputPatternTo.Visible=true; 

            labelNounShowFrom.Visible=true;
            labelNounShowTo.Visible=true;
        }
         
        void ListBoxSetCurrentNoun() {
            for (int indexCur=0; indexCur<itemsNounsFiltered.Count; indexCur++) {
                if (itemsNouns[indexCur]==CurrentNoun) { 
                    int indexList=listBoxNoun.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxNoun.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void SaveCurrentNoun() {
            if (CurrentNoun==null) return;
                     
            CurrentNoun.From=textBoxNounFrom.Text;
            CurrentNoun.To=textBoxNounTo.Text;
            
            
            CurrentNoun.PatternFrom=comboBoxNounInputPatternFrom.Text;

          //  CurrentNoun.PatternTo=comboBoxNounInputPatternTo.SelectedText;
            CurrentNoun.PatternTo=comboBoxNounInputPatternTo.Text;
        } 
              
        void SetNoneNoun(){ 
            textBoxNounFrom.Text="";
            textBoxNounTo.Text="";

            comboBoxNounInputPatternFrom.SelectedIndex=-1;
            comboBoxNounInputPatternTo.SelectedIndex=-1;

            textBoxNounFrom.Visible=false;
            textBoxNounTo.Visible=false;
            labelNounFrom.Visible=false;
            labelNounTo.Visible=false;
            comboBoxNounInputPatternTo.Visible=false;
            comboBoxNounInputPatternFrom.Visible=false;
            labelNounShowFrom.Visible=false;
            labelNounShowTo.Visible=false;
            labelNounInputPatternFrom.Visible=false; 
            labelNounInputPatternTo.Visible=false; 
        }
        
        void ClearNoun(){ 
            listBoxNoun.Items.Clear();
            SetNoneNoun();
            itemsNounsFiltered?.Clear();
            itemsNouns?.Clear();
            CurrentNoun=null;
        }
        #endregion

        #region PatternAdjectiveFrom
        void PatternAdjectiveFromListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternAdjectiveFromSaveCurrent();
            
            int index=listBoxPatternAdjectiveFrom.SelectedIndex;
            if (itemsPatternAdjectiveFrom.Count==0) {
                PatternAdjectiveFromSetNone();
                return;
            }
            if (index>=itemsPatternAdjectiveFrom.Count) 
                index=itemsPatternAdjectiveFrom.Count-1;
            if (index<0)
                index=0;
           
            CurrentPatternAdjectiveFrom=itemsPatternAdjectiveFrom[index];
            PatternAdjectiveFromSetCurrent();
            PatternAdjectiveFromSetListBox();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void PatternAdjectiveFromButtonAdd_Click(object sender, EventArgs e) {
            PatternAdjectiveFromAddNewItem();
        }

        void PatternAdjectiveFromButtonRemove_Click(object sender, EventArgs e) {
            PatternAdjectiveFromRemoveItem(CurrentPatternAdjectiveFrom);
            PatternAdjectiveFromTextBoxFilter_TextChanged(null, new EventArgs());
        }
        
        void PatternAdjectiveFromTextBoxFilter_TextChanged(object sender, EventArgs e) {
            PatternAdjectiveFromSaveCurrent();

            // Získej aktuální prvek
            ItemPatternAdjective selectedId=null;
            if (listBoxPatternAdjectiveFrom.SelectedIndex!=-1) {
                selectedId=itemsPatternAdjectivesFromFiltered[listBoxPatternAdjectiveFrom.SelectedIndex];
            }
            
            PatternAdjectiveFromRefreshFilteredList();

            listBoxPatternAdjectiveFrom.Items.Clear();
            for (int i=0; i<itemsPatternAdjectivesFromFiltered.Count; i++) {
                ItemPatternAdjective item = itemsPatternAdjectivesFromFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternAdjectiveFrom.Items.Add(textToAdd);
            }

            //SetListBoxAdjective();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsPatternAdjectivesFromFiltered.Count; i++){
                    if (itemsPatternAdjectivesFromFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxPatternAdjectiveFrom.SelectedIndex=-1;
                    CurrentPatternAdjectiveFrom=null;
                } else listBoxPatternAdjectiveFrom.SelectedIndex=outIndex;
            } else {
                listBoxPatternAdjectiveFrom.SelectedIndex=-1;
                CurrentPatternAdjectiveFrom=null;
            }
            PatternAdjectiveFromSetCurrent();
        }
            
        void PatternAdjectiveFromRemoveCurrent(object sender, EventArgs e) {
            itemsPatternAdjectiveFrom.Remove(CurrentPatternAdjectiveFrom);
        }

        void PatternAdjectiveFromSetListBox() { 
            string filter=textBoxPatternAdjectiveFromFilter.Text;
            //bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxPatternAdjectiveFrom.SelectedIndex;
            listBoxPatternAdjectiveFrom.Items.Clear();
            for (int i=0; i<itemsPatternAdjectivesFromFiltered.Count; i++) {
                ItemPatternAdjective item = itemsPatternAdjectivesFromFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternAdjectiveFrom.Items.Add(textToAdd);
            }

            if (index>=listBoxPatternAdjectiveFrom.Items.Count)index=listBoxPatternAdjectiveFrom.Items.Count-1;
            listBoxPatternAdjectiveFrom.SelectedIndex=index;
        }

        void PatternAdjectiveFromRefreshFilteredList() {
            if (itemsPatternAdjectivesFromFiltered==null) itemsPatternAdjectivesFromFiltered=new List<ItemPatternAdjective>();
            itemsPatternAdjectivesFromFiltered.Clear();
            string filter=textBoxPatternAdjectiveFromFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsPatternAdjectiveFrom.Count; i++) {
                    ItemPatternAdjective item = itemsPatternAdjectiveFrom[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsPatternAdjectivesFromFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsPatternAdjectiveFrom.Count; i++) {
                    ItemPatternAdjective item = itemsPatternAdjectiveFrom[i];
                    itemsPatternAdjectivesFromFiltered.Add(item);
                }
            }
        }
               
        void PatternAdjectiveFromAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternAdjectiveFromSaveCurrent();

            var newItem=new ItemPatternAdjective();
           // newItem.ID=itemsAdjectives.Count;
            itemsPatternAdjectiveFrom.Add(newItem);
            CurrentPatternAdjectiveFrom=newItem;
            PatternAdjectiveFromRefreshFilteredList();
            PatternAdjectiveFromSetListBox(); 
            PatternAdjectiveFromListBoxSetCurrent();
            PatternAdjectiveFromSetCurrent(); 
            
            doingJob=false;
        }
   
        void PatternAdjectiveFromRemoveItem(ItemPatternAdjective item) { 
            Edited=true;
            ChangeCaptionText();
            itemsPatternAdjectiveFrom.Remove(item);
            PatternAdjectiveFromRefreshFilteredList();
            PatternAdjectiveFromSetListBox();
            PatternAdjectiveFromSetCurrent();
        } 
           
        void PatternAdjectiveFromSetCurrent(){
            if (itemsPatternAdjectivesFromFiltered.Count==0) {
                PatternAdjectiveFromSetNone();
                return;
            }

            int index=listBoxPatternAdjectiveFrom.SelectedIndex;
            if (index>=itemsPatternAdjectivesFromFiltered.Count) index=itemsPatternAdjectivesFromFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternAdjectiveFrom=itemsPatternAdjectivesFromFiltered[index];
            textBoxPatternAdjectiveFromName.Text=CurrentPatternAdjectiveFrom.Name;
            textBoxPatternAdjectiveFromStrJ1.Text=CurrentPatternAdjectiveFrom.Middle[0];
            textBoxPatternAdjectiveFromStrJ2.Text=CurrentPatternAdjectiveFrom.Middle[1];
            textBoxPatternAdjectiveFromStrJ3.Text=CurrentPatternAdjectiveFrom.Middle[2];
            textBoxPatternAdjectiveFromStrJ4.Text=CurrentPatternAdjectiveFrom.Middle[3];
            textBoxPatternAdjectiveFromStrJ5.Text=CurrentPatternAdjectiveFrom.Middle[4];
            textBoxPatternAdjectiveFromStrJ6.Text=CurrentPatternAdjectiveFrom.Middle[5];
            textBoxPatternAdjectiveFromStrJ7.Text=CurrentPatternAdjectiveFrom.Middle[6];

            textBoxPatternAdjectiveFromStrM1.Text=CurrentPatternAdjectiveFrom.Middle[7];
            textBoxPatternAdjectiveFromStrM2.Text=CurrentPatternAdjectiveFrom.Middle[8];
            textBoxPatternAdjectiveFromStrM3.Text=CurrentPatternAdjectiveFrom.Middle[9];
            textBoxPatternAdjectiveFromStrM4.Text=CurrentPatternAdjectiveFrom.Middle[10];
            textBoxPatternAdjectiveFromStrM5.Text=CurrentPatternAdjectiveFrom.Middle[11];
            textBoxPatternAdjectiveFromStrM6.Text=CurrentPatternAdjectiveFrom.Middle[12];
            textBoxPatternAdjectiveFromStrM7.Text=CurrentPatternAdjectiveFrom.Middle[13];

            textBoxPatternAdjectiveFromZenJ1.Text=CurrentPatternAdjectiveFrom.Feminine[0];
            textBoxPatternAdjectiveFromZenJ2.Text=CurrentPatternAdjectiveFrom.Feminine[1];
            textBoxPatternAdjectiveFromZenJ3.Text=CurrentPatternAdjectiveFrom.Feminine[2];
            textBoxPatternAdjectiveFromZenJ4.Text=CurrentPatternAdjectiveFrom.Feminine[3];
            textBoxPatternAdjectiveFromZenJ5.Text=CurrentPatternAdjectiveFrom.Feminine[4];
            textBoxPatternAdjectiveFromZenJ6.Text=CurrentPatternAdjectiveFrom.Feminine[5];
            textBoxPatternAdjectiveFromZenJ7.Text=CurrentPatternAdjectiveFrom.Feminine[6];

            textBoxPatternAdjectiveFromZenM1.Text=CurrentPatternAdjectiveFrom.Feminine[7];
            textBoxPatternAdjectiveFromZenM2.Text=CurrentPatternAdjectiveFrom.Feminine[8];
            textBoxPatternAdjectiveFromZenM3.Text=CurrentPatternAdjectiveFrom.Feminine[9];
            textBoxPatternAdjectiveFromZenM4.Text=CurrentPatternAdjectiveFrom.Feminine[10];
            textBoxPatternAdjectiveFromZenM5.Text=CurrentPatternAdjectiveFrom.Feminine[11];
            textBoxPatternAdjectiveFromZenM6.Text=CurrentPatternAdjectiveFrom.Feminine[12];
            textBoxPatternAdjectiveFromZenM7.Text=CurrentPatternAdjectiveFrom.Feminine[13];

            textBoxPatternAdjectiveFromMuzJ1.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[0];
            textBoxPatternAdjectiveFromMuzJ2.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[1];
            textBoxPatternAdjectiveFromMuzJ3.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[2];
            textBoxPatternAdjectiveFromMuzJ4.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[3];
            textBoxPatternAdjectiveFromMuzJ5.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[4];
            textBoxPatternAdjectiveFromMuzJ6.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[5];
            textBoxPatternAdjectiveFromMuzJ7.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[6];

            textBoxPatternAdjectiveFromMuzM1.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[7];
            textBoxPatternAdjectiveFromMuzM2.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[8];
            textBoxPatternAdjectiveFromMuzM3.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[9];
            textBoxPatternAdjectiveFromMuzM4.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[10];
            textBoxPatternAdjectiveFromMuzM5.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[11];
            textBoxPatternAdjectiveFromMuzM6.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[12];
            textBoxPatternAdjectiveFromMuzM7.Text=CurrentPatternAdjectiveFrom.MasculineAnimate[13];

            textBoxPatternAdjectiveFromMunJ1.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[0];
            textBoxPatternAdjectiveFromMunJ2.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[1];
            textBoxPatternAdjectiveFromMunJ3.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[2];
            textBoxPatternAdjectiveFromMunJ4.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[3];
            textBoxPatternAdjectiveFromMunJ5.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[4];
            textBoxPatternAdjectiveFromMunJ6.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[5];
            textBoxPatternAdjectiveFromMunJ7.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[6];

            textBoxPatternAdjectiveFromMunM1.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[7];
            textBoxPatternAdjectiveFromMunM2.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[8];
            textBoxPatternAdjectiveFromMunM3.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[9];
            textBoxPatternAdjectiveFromMunM4.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[10];
            textBoxPatternAdjectiveFromMunM5.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[11];
            textBoxPatternAdjectiveFromMunM6.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[12];
            textBoxPatternAdjectiveFromMunM7.Text=CurrentPatternAdjectiveFrom.MasculineInanimate[13];

            
            textBoxPatternAdjectiveFromName.Visible=true;
            tableLayoutPanelPatternAdjectiveFromStr.Visible=true;
            labelPatternAdjectiveFromStrFall.Visible=true;
            labelPatternAdjectiveFromStrMultiple.Visible=true;
            labelPatternAdjectiveFromStrSingle.Visible=true;

            tableLayoutPanelPatternAdjectiveFromMuz.Visible=true;
            labelPatternAdjectiveFromMuzFall.Visible=true;
            labelPatternAdjectiveFromMuzMultiple.Visible=true;
            labelPatternAdjectiveFromMuzSingle.Visible=true;

            tableLayoutPanelPatternAdjectiveFromMun.Visible=true;
            labelPatternAdjectiveFromMunFall.Visible=true;
            labelPatternAdjectiveFromMunMultiple.Visible=true;
            labelPatternAdjectiveFromMunSingle.Visible=true;

            tableLayoutPanelPatternAdjectiveFromZen.Visible=true;
            labelPatternAdjectiveFromZenFall.Visible=true;
            labelPatternAdjectiveFromZenMultiple.Visible=true;
            labelPatternAdjectiveFromZenSingle.Visible=true;

            labelPatternAdjectiveFromName.Visible=true;
            //labelPatternAdjectiveType.Visible=true;
           // comboBoxPatternAdjectiveFromType.Visible=true;
            labelPatternAdjectiveFromStr.Visible=true;
            labelPatternAdjectiveFromZen.Visible=true;
            labelPatternAdjectiveFromMuz.Visible=true;
            labelPatternAdjectiveFromMun.Visible=true;
          //  comboBoxPatternAdjectiveFromType.SelectedIndex=(int)CurrentPatternAdjectiveFrom.adjectiveType;
        }
         
        void PatternAdjectiveFromListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternAdjectivesFromFiltered.Count; indexCur++) {
                if (itemsPatternAdjectiveFrom[indexCur]==CurrentPatternAdjectiveFrom) { 
                    int indexList=listBoxPatternAdjectiveFrom.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxPatternAdjectiveFrom.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void PatternAdjectiveFromSaveCurrent() {
            if (CurrentPatternAdjectiveFrom==null) return;
                     
            CurrentPatternAdjectiveFrom.Name=textBoxPatternAdjectiveFromName.Text;

            CurrentPatternAdjectiveFrom.Middle[0]=textBoxPatternAdjectiveFromStrJ1.Text;
            CurrentPatternAdjectiveFrom.Middle[1]=textBoxPatternAdjectiveFromStrJ2.Text;
            CurrentPatternAdjectiveFrom.Middle[2]=textBoxPatternAdjectiveFromStrJ3.Text;
            CurrentPatternAdjectiveFrom.Middle[3]=textBoxPatternAdjectiveFromStrJ4.Text;
            CurrentPatternAdjectiveFrom.Middle[4]=textBoxPatternAdjectiveFromStrJ5.Text;
            CurrentPatternAdjectiveFrom.Middle[5]=textBoxPatternAdjectiveFromStrJ6.Text;
            CurrentPatternAdjectiveFrom.Middle[6]=textBoxPatternAdjectiveFromStrJ7.Text;

            CurrentPatternAdjectiveFrom.Middle[ 7]=textBoxPatternAdjectiveFromStrM1.Text;
            CurrentPatternAdjectiveFrom.Middle[ 8]=textBoxPatternAdjectiveFromStrM2.Text;
            CurrentPatternAdjectiveFrom.Middle[ 9]=textBoxPatternAdjectiveFromStrM3.Text;
            CurrentPatternAdjectiveFrom.Middle[10]=textBoxPatternAdjectiveFromStrM4.Text;
            CurrentPatternAdjectiveFrom.Middle[11]=textBoxPatternAdjectiveFromStrM5.Text;
            CurrentPatternAdjectiveFrom.Middle[12]=textBoxPatternAdjectiveFromStrM6.Text;
            CurrentPatternAdjectiveFrom.Middle[13]=textBoxPatternAdjectiveFromStrM7.Text;

            CurrentPatternAdjectiveFrom.Feminine[0]=textBoxPatternAdjectiveFromZenJ1.Text;
            CurrentPatternAdjectiveFrom.Feminine[1]=textBoxPatternAdjectiveFromZenJ2.Text;
            CurrentPatternAdjectiveFrom.Feminine[2]=textBoxPatternAdjectiveFromZenJ3.Text;
            CurrentPatternAdjectiveFrom.Feminine[3]=textBoxPatternAdjectiveFromZenJ4.Text;
            CurrentPatternAdjectiveFrom.Feminine[4]=textBoxPatternAdjectiveFromZenJ5.Text;
            CurrentPatternAdjectiveFrom.Feminine[5]=textBoxPatternAdjectiveFromZenJ6.Text;
            CurrentPatternAdjectiveFrom.Feminine[6]=textBoxPatternAdjectiveFromZenJ7.Text;

            CurrentPatternAdjectiveFrom.Feminine[ 7]=textBoxPatternAdjectiveFromZenM1.Text;
            CurrentPatternAdjectiveFrom.Feminine[ 8]=textBoxPatternAdjectiveFromZenM2.Text;
            CurrentPatternAdjectiveFrom.Feminine[ 9]=textBoxPatternAdjectiveFromZenM3.Text;
            CurrentPatternAdjectiveFrom.Feminine[10]=textBoxPatternAdjectiveFromZenM4.Text;
            CurrentPatternAdjectiveFrom.Feminine[11]=textBoxPatternAdjectiveFromZenM5.Text;
            CurrentPatternAdjectiveFrom.Feminine[12]=textBoxPatternAdjectiveFromZenM6.Text;
            CurrentPatternAdjectiveFrom.Feminine[13]=textBoxPatternAdjectiveFromZenM7.Text;

            CurrentPatternAdjectiveFrom.MasculineAnimate[0]=textBoxPatternAdjectiveFromMuzJ1.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[1]=textBoxPatternAdjectiveFromMuzJ2.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[2]=textBoxPatternAdjectiveFromMuzJ3.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[3]=textBoxPatternAdjectiveFromMuzJ4.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[4]=textBoxPatternAdjectiveFromMuzJ5.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[5]=textBoxPatternAdjectiveFromMuzJ6.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[6]=textBoxPatternAdjectiveFromMuzJ7.Text;

            CurrentPatternAdjectiveFrom.MasculineAnimate[7]=textBoxPatternAdjectiveFromMuzM1.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[8]=textBoxPatternAdjectiveFromMuzM2.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[9]=textBoxPatternAdjectiveFromMuzM3.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[10]=textBoxPatternAdjectiveFromMuzM4.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[11]=textBoxPatternAdjectiveFromMuzM5.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[12]=textBoxPatternAdjectiveFromMuzM6.Text;
            CurrentPatternAdjectiveFrom.MasculineAnimate[13]=textBoxPatternAdjectiveFromMuzM7.Text;

            CurrentPatternAdjectiveFrom.MasculineInanimate[0]=textBoxPatternAdjectiveFromMunJ1.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[1]=textBoxPatternAdjectiveFromMunJ2.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[2]=textBoxPatternAdjectiveFromMunJ3.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[3]=textBoxPatternAdjectiveFromMunJ4.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[4]=textBoxPatternAdjectiveFromMunJ5.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[5]=textBoxPatternAdjectiveFromMunJ6.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[6]=textBoxPatternAdjectiveFromMunJ7.Text;

            CurrentPatternAdjectiveFrom.MasculineInanimate[ 7]=textBoxPatternAdjectiveFromMunM1.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[ 8]=textBoxPatternAdjectiveFromMunM2.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[ 9]=textBoxPatternAdjectiveFromMunM3.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[10]=textBoxPatternAdjectiveFromMunM4.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[11]=textBoxPatternAdjectiveFromMunM5.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[12]=textBoxPatternAdjectiveFromMunM6.Text;
            CurrentPatternAdjectiveFrom.MasculineInanimate[13]=textBoxPatternAdjectiveFromMunM7.Text;
        } 
              
        void PatternAdjectiveFromSetNone(){ 
            textBoxPatternAdjectiveFromName.Text="";

            textBoxPatternAdjectiveFromStrJ1.Text="";
            textBoxPatternAdjectiveFromStrJ2.Text="";
            textBoxPatternAdjectiveFromStrJ3.Text="";
            textBoxPatternAdjectiveFromStrJ4.Text="";
            textBoxPatternAdjectiveFromStrJ5.Text="";
            textBoxPatternAdjectiveFromStrJ6.Text="";
            textBoxPatternAdjectiveFromStrJ7.Text="";

            textBoxPatternAdjectiveFromStrM1.Text="";
            textBoxPatternAdjectiveFromStrM2.Text="";
            textBoxPatternAdjectiveFromStrM3.Text="";
            textBoxPatternAdjectiveFromStrM4.Text="";
            textBoxPatternAdjectiveFromStrM5.Text="";
            textBoxPatternAdjectiveFromStrM6.Text="";
            textBoxPatternAdjectiveFromStrM7.Text="";

            textBoxPatternAdjectiveFromZenJ1.Text="";
            textBoxPatternAdjectiveFromZenJ2.Text="";
            textBoxPatternAdjectiveFromZenJ3.Text="";
            textBoxPatternAdjectiveFromZenJ4.Text="";
            textBoxPatternAdjectiveFromZenJ5.Text="";
            textBoxPatternAdjectiveFromZenJ6.Text="";
            textBoxPatternAdjectiveFromZenJ7.Text="";

            textBoxPatternAdjectiveFromZenM1.Text="";
            textBoxPatternAdjectiveFromZenM2.Text="";
            textBoxPatternAdjectiveFromZenM3.Text="";
            textBoxPatternAdjectiveFromZenM4.Text="";
            textBoxPatternAdjectiveFromZenM5.Text="";
            textBoxPatternAdjectiveFromZenM6.Text="";
            textBoxPatternAdjectiveFromZenM7.Text="";

            textBoxPatternAdjectiveFromMuzJ1.Text="";
            textBoxPatternAdjectiveFromMuzJ2.Text="";
            textBoxPatternAdjectiveFromMuzJ3.Text="";
            textBoxPatternAdjectiveFromMuzJ4.Text="";
            textBoxPatternAdjectiveFromMuzJ5.Text="";
            textBoxPatternAdjectiveFromMuzJ6.Text="";
            textBoxPatternAdjectiveFromMuzJ7.Text="";

            textBoxPatternAdjectiveFromMuzM1.Text="";
            textBoxPatternAdjectiveFromMuzM2.Text="";
            textBoxPatternAdjectiveFromMuzM3.Text="";
            textBoxPatternAdjectiveFromMuzM4.Text="";
            textBoxPatternAdjectiveFromMuzM5.Text="";
            textBoxPatternAdjectiveFromMuzM6.Text="";
            textBoxPatternAdjectiveFromMuzM7.Text="";

            textBoxPatternAdjectiveFromMunJ1.Text="";
            textBoxPatternAdjectiveFromMunJ2.Text="";
            textBoxPatternAdjectiveFromMunJ3.Text="";
            textBoxPatternAdjectiveFromMunJ4.Text="";
            textBoxPatternAdjectiveFromMunJ5.Text="";
            textBoxPatternAdjectiveFromMunJ6.Text="";
            textBoxPatternAdjectiveFromMunJ7.Text="";

            textBoxPatternAdjectiveFromMunM1.Text="";
            textBoxPatternAdjectiveFromMunM2.Text="";
            textBoxPatternAdjectiveFromMunM3.Text="";
            textBoxPatternAdjectiveFromMunM4.Text="";
            textBoxPatternAdjectiveFromMunM5.Text="";
            textBoxPatternAdjectiveFromMunM6.Text="";
            textBoxPatternAdjectiveFromMunM7.Text="";
            textBoxPatternAdjectiveFromName.Visible=false;
            labelPatternAdjectiveFromStr.Visible=false;
            tableLayoutPanelPatternAdjectiveFromStr.Visible=false;
            labelPatternAdjectiveFromStrFall.Visible=false;
            labelPatternAdjectiveFromStrMultiple.Visible=false;
            labelPatternAdjectiveFromStrSingle.Visible=false;

            tableLayoutPanelPatternAdjectiveFromMuz.Visible=false;
            labelPatternAdjectiveFromMuzFall.Visible=false;
            labelPatternAdjectiveFromMuzMultiple.Visible=false;
            labelPatternAdjectiveFromMuzSingle.Visible=false;
            labelPatternAdjectiveFromMuz.Visible=false;
            tableLayoutPanelPatternAdjectiveFromMun.Visible=false;
            labelPatternAdjectiveFromMunFall.Visible=false;
            labelPatternAdjectiveFromMunMultiple.Visible=false;
            labelPatternAdjectiveFromMunSingle.Visible=false;
            labelPatternAdjectiveFromZen.Visible=false;
            tableLayoutPanelPatternAdjectiveFromZen.Visible=false;
            labelPatternAdjectiveFromZenFall.Visible=false;
            labelPatternAdjectiveFromZenMultiple.Visible=false;
            labelPatternAdjectiveFromZenSingle.Visible=false;
            labelPatternAdjectiveFromMun.Visible=false;
            labelPatternAdjectiveFromName.Visible=false;
        }
        
        void PatternAdjectiveFromClear() { 
            listBoxPatternAdjectiveFrom.Items.Clear();
            PatternAdjectiveFromSetNone();
            itemsPatternAdjectivesFromFiltered?.Clear();
            itemsPatternAdjectiveFrom?.Clear();
            CurrentPatternAdjectiveFrom=null;
        }
        #endregion
        
        #region PatternAdjectiveTo
        void PatternAdjectiveToListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternAdjectiveToSaveCurrent();
            
            int index=listBoxPatternAdjectiveTo.SelectedIndex;
            if (itemsPatternAdjectiveTo.Count==0) {
                PatternAdjectiveToSetNone();
                return;
            }
            if (index>=itemsPatternAdjectiveTo.Count) 
                index=itemsPatternAdjectiveTo.Count-1;
            if (index<0)
                index=0;
           
            CurrentPatternAdjectiveTo=itemsPatternAdjectiveTo[index];
            PatternAdjectiveToSetCurrent();
            PatternAdjectiveToSetListBox();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void PatternAdjectiveToButtonAdd_Click(object sender, EventArgs e) {
            PatternAdjectiveToAddNewItem();
        }

        void PatternAdjectiveToButtonRemove_Click(object sender, EventArgs e) {
            PatternAdjectiveToRemoveItem(CurrentPatternAdjectiveTo);
            PatternAdjectiveToTextBoxFilter_TextChanged(null, new EventArgs());
        }
        
        void PatternAdjectiveToTextBoxFilter_TextChanged(object sender, EventArgs e) {
            PatternAdjectiveToSaveCurrent();

            // Získej aktuální prvek
            ItemPatternAdjective selectedId=null;
            if (listBoxPatternAdjectiveTo.SelectedIndex!=-1) {
                selectedId=itemsPatternAdjectivesToFiltered[listBoxPatternAdjectiveTo.SelectedIndex];
            }
            
            PatternAdjectiveToRefreshFilteredList();

            listBoxPatternAdjectiveTo.Items.Clear();
            for (int i=0; i<itemsPatternAdjectivesToFiltered.Count; i++) {
                ItemPatternAdjective item = itemsPatternAdjectivesToFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternAdjectiveTo.Items.Add(textToAdd);
            }

            //SetListBoxAdjective();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsPatternAdjectivesToFiltered.Count; i++){
                    if (itemsPatternAdjectivesToFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxPatternAdjectiveTo.SelectedIndex=-1;
                    CurrentPatternAdjectiveTo=null;
                } else listBoxPatternAdjectiveTo.SelectedIndex=outIndex;
            } else {
                listBoxPatternAdjectiveTo.SelectedIndex=-1;
                CurrentPatternAdjectiveTo=null;
            }
            PatternAdjectiveToSetCurrent();
        }
            
        void PatternAdjectiveToRemoveCurrent(object sender, EventArgs e) {
            itemsPatternAdjectiveTo.Remove(CurrentPatternAdjectiveTo);
        }

        void PatternAdjectiveToSetListBox() { 
            string filter=textBoxPatternAdjectiveToFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxPatternAdjectiveTo.SelectedIndex;
            listBoxPatternAdjectiveTo.Items.Clear();
            for (int i=0; i<itemsPatternAdjectivesToFiltered.Count; i++) {
                ItemPatternAdjective item = itemsPatternAdjectivesToFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternAdjectiveTo.Items.Add(textToAdd);
            }

            if (index>=listBoxPatternAdjectiveTo.Items.Count)index=listBoxPatternAdjectiveTo.Items.Count-1;
            listBoxPatternAdjectiveTo.SelectedIndex=index;
        }

        void PatternAdjectiveToRefreshFilteredList() {
            if (itemsPatternAdjectivesToFiltered==null) itemsPatternAdjectivesToFiltered=new List<ItemPatternAdjective>();
            itemsPatternAdjectivesToFiltered.Clear();
            string filter=textBoxPatternAdjectiveToFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsPatternAdjectiveTo.Count; i++) {
                    ItemPatternAdjective item = itemsPatternAdjectiveTo[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsPatternAdjectivesToFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsPatternAdjectiveTo.Count; i++) {
                    ItemPatternAdjective item = itemsPatternAdjectiveTo[i];
                    itemsPatternAdjectivesToFiltered.Add(item);
                }
            }
        }
               
        void PatternAdjectiveToAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternAdjectiveToSaveCurrent();

            var newItem=new ItemPatternAdjective();
           // newItem.ID=itemsAdjectives.Count;
            itemsPatternAdjectiveTo.Add(newItem);
            CurrentPatternAdjectiveTo=newItem;
            PatternAdjectiveToRefreshFilteredList();
            PatternAdjectiveToSetListBox(); 
            PatternAdjectiveToListBoxSetCurrent();
            PatternAdjectiveToSetCurrent(); 
            
            doingJob=false;
        }
   
        void PatternAdjectiveToRemoveItem(ItemPatternAdjective item) { 
            Edited=true;
            ChangeCaptionText();
            itemsPatternAdjectiveTo.Remove(item);
            PatternAdjectiveToRefreshFilteredList();
            PatternAdjectiveToSetListBox();
            PatternAdjectiveToSetCurrent();
        } 
           
        void PatternAdjectiveToSetCurrent(){
            if (itemsPatternAdjectivesToFiltered.Count==0) {
                PatternAdjectiveToSetNone();
                return;
            }

            int index=listBoxPatternAdjectiveTo.SelectedIndex;
            if (index>=itemsPatternAdjectivesToFiltered.Count) index=itemsPatternAdjectivesToFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternAdjectiveTo=itemsPatternAdjectivesToFiltered[index];
            textBoxPatternAdjectiveToName.Text=CurrentPatternAdjectiveTo.Name;
            textBoxPatternAdjectiveToStrJ1.Text=CurrentPatternAdjectiveTo.Middle[0];
            textBoxPatternAdjectiveToStrJ2.Text=CurrentPatternAdjectiveTo.Middle[1];
            textBoxPatternAdjectiveToStrJ3.Text=CurrentPatternAdjectiveTo.Middle[2];
            textBoxPatternAdjectiveToStrJ4.Text=CurrentPatternAdjectiveTo.Middle[3];
            textBoxPatternAdjectiveToStrJ5.Text=CurrentPatternAdjectiveTo.Middle[4];
            textBoxPatternAdjectiveToStrJ6.Text=CurrentPatternAdjectiveTo.Middle[5];
            textBoxPatternAdjectiveToStrJ7.Text=CurrentPatternAdjectiveTo.Middle[6];

            textBoxPatternAdjectiveToStrM1.Text=CurrentPatternAdjectiveTo.Middle[7];
            textBoxPatternAdjectiveToStrM2.Text=CurrentPatternAdjectiveTo.Middle[8];
            textBoxPatternAdjectiveToStrM3.Text=CurrentPatternAdjectiveTo.Middle[9];
            textBoxPatternAdjectiveToStrM4.Text=CurrentPatternAdjectiveTo.Middle[10];
            textBoxPatternAdjectiveToStrM5.Text=CurrentPatternAdjectiveTo.Middle[11];
            textBoxPatternAdjectiveToStrM6.Text=CurrentPatternAdjectiveTo.Middle[12];
            textBoxPatternAdjectiveToStrM7.Text=CurrentPatternAdjectiveTo.Middle[13];

            textBoxPatternAdjectiveToZenJ1.Text=CurrentPatternAdjectiveTo.Feminine[0];
            textBoxPatternAdjectiveToZenJ2.Text=CurrentPatternAdjectiveTo.Feminine[1];
            textBoxPatternAdjectiveToZenJ3.Text=CurrentPatternAdjectiveTo.Feminine[2];
            textBoxPatternAdjectiveToZenJ4.Text=CurrentPatternAdjectiveTo.Feminine[3];
            textBoxPatternAdjectiveToZenJ5.Text=CurrentPatternAdjectiveTo.Feminine[4];
            textBoxPatternAdjectiveToZenJ6.Text=CurrentPatternAdjectiveTo.Feminine[5];
            textBoxPatternAdjectiveToZenJ7.Text=CurrentPatternAdjectiveTo.Feminine[6];

            textBoxPatternAdjectiveToZenM1.Text=CurrentPatternAdjectiveTo.Feminine[7];
            textBoxPatternAdjectiveToZenM2.Text=CurrentPatternAdjectiveTo.Feminine[8];
            textBoxPatternAdjectiveToZenM3.Text=CurrentPatternAdjectiveTo.Feminine[9];
            textBoxPatternAdjectiveToZenM4.Text=CurrentPatternAdjectiveTo.Feminine[10];
            textBoxPatternAdjectiveToZenM5.Text=CurrentPatternAdjectiveTo.Feminine[11];
            textBoxPatternAdjectiveToZenM6.Text=CurrentPatternAdjectiveTo.Feminine[12];
            textBoxPatternAdjectiveToZenM7.Text=CurrentPatternAdjectiveTo.Feminine[13];

            textBoxPatternAdjectiveToMuzJ1.Text=CurrentPatternAdjectiveTo.MasculineAnimate[0];
            textBoxPatternAdjectiveToMuzJ2.Text=CurrentPatternAdjectiveTo.MasculineAnimate[1];
            textBoxPatternAdjectiveToMuzJ3.Text=CurrentPatternAdjectiveTo.MasculineAnimate[2];
            textBoxPatternAdjectiveToMuzJ4.Text=CurrentPatternAdjectiveTo.MasculineAnimate[3];
            textBoxPatternAdjectiveToMuzJ5.Text=CurrentPatternAdjectiveTo.MasculineAnimate[4];
            textBoxPatternAdjectiveToMuzJ6.Text=CurrentPatternAdjectiveTo.MasculineAnimate[5];
            textBoxPatternAdjectiveToMuzJ7.Text=CurrentPatternAdjectiveTo.MasculineAnimate[6];

            textBoxPatternAdjectiveToMuzM1.Text=CurrentPatternAdjectiveTo.MasculineAnimate[7];
            textBoxPatternAdjectiveToMuzM2.Text=CurrentPatternAdjectiveTo.MasculineAnimate[8];
            textBoxPatternAdjectiveToMuzM3.Text=CurrentPatternAdjectiveTo.MasculineAnimate[9];
            textBoxPatternAdjectiveToMuzM4.Text=CurrentPatternAdjectiveTo.MasculineAnimate[10];
            textBoxPatternAdjectiveToMuzM5.Text=CurrentPatternAdjectiveTo.MasculineAnimate[11];
            textBoxPatternAdjectiveToMuzM6.Text=CurrentPatternAdjectiveTo.MasculineAnimate[12];
            textBoxPatternAdjectiveToMuzM7.Text=CurrentPatternAdjectiveTo.MasculineAnimate[13];

            textBoxPatternAdjectiveToMunJ1.Text=CurrentPatternAdjectiveTo.MasculineInanimate[0];
            textBoxPatternAdjectiveToMunJ2.Text=CurrentPatternAdjectiveTo.MasculineInanimate[1];
            textBoxPatternAdjectiveToMunJ3.Text=CurrentPatternAdjectiveTo.MasculineInanimate[2];
            textBoxPatternAdjectiveToMunJ4.Text=CurrentPatternAdjectiveTo.MasculineInanimate[3];
            textBoxPatternAdjectiveToMunJ5.Text=CurrentPatternAdjectiveTo.MasculineInanimate[4];
            textBoxPatternAdjectiveToMunJ6.Text=CurrentPatternAdjectiveTo.MasculineInanimate[5];
            textBoxPatternAdjectiveToMunJ7.Text=CurrentPatternAdjectiveTo.MasculineInanimate[6];

            textBoxPatternAdjectiveToMunM1.Text=CurrentPatternAdjectiveTo.MasculineInanimate[7];
            textBoxPatternAdjectiveToMunM2.Text=CurrentPatternAdjectiveTo.MasculineInanimate[8];
            textBoxPatternAdjectiveToMunM3.Text=CurrentPatternAdjectiveTo.MasculineInanimate[9];
            textBoxPatternAdjectiveToMunM4.Text=CurrentPatternAdjectiveTo.MasculineInanimate[10];
            textBoxPatternAdjectiveToMunM5.Text=CurrentPatternAdjectiveTo.MasculineInanimate[11];
            textBoxPatternAdjectiveToMunM6.Text=CurrentPatternAdjectiveTo.MasculineInanimate[12];
            textBoxPatternAdjectiveToMunM7.Text=CurrentPatternAdjectiveTo.MasculineInanimate[13];

            
            textBoxPatternAdjectiveToName.Visible=true;
            tableLayoutPanelPatternAdjectiveFromStr.Visible=true;
            labelPatternAdjectiveFromStrFall.Visible=true;
            labelPatternAdjectiveFromStrMultiple.Visible=true;
            labelPatternAdjectiveFromStrSingle.Visible=true;

            tableLayoutPanelPatternAdjectiveFromMuz.Visible=true;
            labelPatternAdjectiveFromMuzFall.Visible=true;
            labelPatternAdjectiveFromMuzMultiple.Visible=true;
            labelPatternAdjectiveFromMuzSingle.Visible=true;

            tableLayoutPanelPatternAdjectiveFromMun.Visible=true;
            labelPatternAdjectiveToMunFall.Visible=true;
            labelPatternAdjectiveFromMunMultiple.Visible=true;
            labelPatternAdjectiveFromMunSingle.Visible=true;

            tableLayoutPanelPatternAdjectiveFromZen.Visible=true;
            labelPatternAdjectiveFromZenFall.Visible=true;
            labelPatternAdjectiveFromZenMultiple.Visible=true;
            labelPatternAdjectiveFromZenSingle.Visible=true;

            labelPatternAdjectiveFromName.Visible=true;
            labelPatternAdjectiveToStr.Visible=true;
            labelPatternAdjectiveFromZen.Visible=true;
            labelPatternAdjectiveFromMuz.Visible=true;
            labelPatternAdjectiveToMun.Visible=true;
        }
         
        void PatternAdjectiveToListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternAdjectivesToFiltered.Count; indexCur++) {
                if (itemsPatternAdjectiveTo[indexCur]==CurrentPatternAdjectiveTo) { 
                    int indexList=listBoxPatternAdjectiveTo.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxPatternAdjectiveTo.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void PatternAdjectiveToSaveCurrent() {
            if (CurrentPatternAdjectiveTo==null) return;
                     
            CurrentPatternAdjectiveTo.Name=textBoxPatternAdjectiveToName.Text;

            CurrentPatternAdjectiveTo.Middle[0]=textBoxPatternAdjectiveToStrJ1.Text;
            CurrentPatternAdjectiveTo.Middle[1]=textBoxPatternAdjectiveToStrJ2.Text;
            CurrentPatternAdjectiveTo.Middle[2]=textBoxPatternAdjectiveToStrJ3.Text;
            CurrentPatternAdjectiveTo.Middle[3]=textBoxPatternAdjectiveToStrJ4.Text;
            CurrentPatternAdjectiveTo.Middle[4]=textBoxPatternAdjectiveToStrJ5.Text;
            CurrentPatternAdjectiveTo.Middle[5]=textBoxPatternAdjectiveToStrJ6.Text;
            CurrentPatternAdjectiveTo.Middle[6]=textBoxPatternAdjectiveToStrJ7.Text;

            CurrentPatternAdjectiveTo.Middle[ 7]=textBoxPatternAdjectiveToStrM1.Text;
            CurrentPatternAdjectiveTo.Middle[ 8]=textBoxPatternAdjectiveToStrM2.Text;
            CurrentPatternAdjectiveTo.Middle[ 9]=textBoxPatternAdjectiveToStrM3.Text;
            CurrentPatternAdjectiveTo.Middle[10]=textBoxPatternAdjectiveToStrM4.Text;
            CurrentPatternAdjectiveTo.Middle[11]=textBoxPatternAdjectiveToStrM5.Text;
            CurrentPatternAdjectiveTo.Middle[12]=textBoxPatternAdjectiveToStrM6.Text;
            CurrentPatternAdjectiveTo.Middle[13]=textBoxPatternAdjectiveToStrM7.Text;

            CurrentPatternAdjectiveTo.Feminine[0]=textBoxPatternAdjectiveToZenJ1.Text;
            CurrentPatternAdjectiveTo.Feminine[1]=textBoxPatternAdjectiveToZenJ2.Text;
            CurrentPatternAdjectiveTo.Feminine[2]=textBoxPatternAdjectiveToZenJ3.Text;
            CurrentPatternAdjectiveTo.Feminine[3]=textBoxPatternAdjectiveToZenJ4.Text;
            CurrentPatternAdjectiveTo.Feminine[4]=textBoxPatternAdjectiveToZenJ5.Text;
            CurrentPatternAdjectiveTo.Feminine[5]=textBoxPatternAdjectiveToZenJ6.Text;
            CurrentPatternAdjectiveTo.Feminine[6]=textBoxPatternAdjectiveToZenJ7.Text;

            CurrentPatternAdjectiveTo.Feminine[ 7]=textBoxPatternAdjectiveToZenM1.Text;
            CurrentPatternAdjectiveTo.Feminine[ 8]=textBoxPatternAdjectiveToZenM2.Text;
            CurrentPatternAdjectiveTo.Feminine[ 9]=textBoxPatternAdjectiveToZenM3.Text;
            CurrentPatternAdjectiveTo.Feminine[10]=textBoxPatternAdjectiveToZenM4.Text;
            CurrentPatternAdjectiveTo.Feminine[11]=textBoxPatternAdjectiveToZenM5.Text;
            CurrentPatternAdjectiveTo.Feminine[12]=textBoxPatternAdjectiveToZenM6.Text;
            CurrentPatternAdjectiveTo.Feminine[13]=textBoxPatternAdjectiveToZenM7.Text;

            CurrentPatternAdjectiveTo.MasculineAnimate[0]=textBoxPatternAdjectiveToMuzJ1.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[1]=textBoxPatternAdjectiveToMuzJ2.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[2]=textBoxPatternAdjectiveToMuzJ3.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[3]=textBoxPatternAdjectiveToMuzJ4.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[4]=textBoxPatternAdjectiveToMuzJ5.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[5]=textBoxPatternAdjectiveToMuzJ6.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[6]=textBoxPatternAdjectiveToMuzJ7.Text;

            CurrentPatternAdjectiveTo.MasculineAnimate[ 7]=textBoxPatternAdjectiveToMuzM1.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[ 8]=textBoxPatternAdjectiveToMuzM2.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[ 9]=textBoxPatternAdjectiveToMuzM3.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[10]=textBoxPatternAdjectiveToMuzM4.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[11]=textBoxPatternAdjectiveToMuzM5.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[12]=textBoxPatternAdjectiveToMuzM6.Text;
            CurrentPatternAdjectiveTo.MasculineAnimate[13]=textBoxPatternAdjectiveToMuzM7.Text;

            CurrentPatternAdjectiveTo.MasculineInanimate[0]=textBoxPatternAdjectiveToMunJ1.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[1]=textBoxPatternAdjectiveToMunJ2.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[2]=textBoxPatternAdjectiveToMunJ3.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[3]=textBoxPatternAdjectiveToMunJ4.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[4]=textBoxPatternAdjectiveToMunJ5.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[5]=textBoxPatternAdjectiveToMunJ6.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[6]=textBoxPatternAdjectiveToMunJ7.Text;

            CurrentPatternAdjectiveTo.MasculineInanimate[ 7]=textBoxPatternAdjectiveToMunM1.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[ 8]=textBoxPatternAdjectiveToMunM2.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[ 9]=textBoxPatternAdjectiveToMunM3.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[10]=textBoxPatternAdjectiveToMunM4.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[11]=textBoxPatternAdjectiveToMunM5.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[12]=textBoxPatternAdjectiveToMunM6.Text;
            CurrentPatternAdjectiveTo.MasculineInanimate[13]=textBoxPatternAdjectiveToMunM7.Text;
        } 
              
        void PatternAdjectiveToSetNone(){ 
            textBoxPatternAdjectiveToName.Text="";

            textBoxPatternAdjectiveToStrJ1.Text="";
            textBoxPatternAdjectiveToStrJ2.Text="";
            textBoxPatternAdjectiveToStrJ3.Text="";
            textBoxPatternAdjectiveToStrJ4.Text="";
            textBoxPatternAdjectiveToStrJ5.Text="";
            textBoxPatternAdjectiveToStrJ6.Text="";
            textBoxPatternAdjectiveToStrJ7.Text="";

            textBoxPatternAdjectiveToStrM1.Text="";
            textBoxPatternAdjectiveToStrM2.Text="";
            textBoxPatternAdjectiveToStrM3.Text="";
            textBoxPatternAdjectiveToStrM4.Text="";
            textBoxPatternAdjectiveToStrM5.Text="";
            textBoxPatternAdjectiveToStrM6.Text="";
            textBoxPatternAdjectiveToStrM7.Text="";

            textBoxPatternAdjectiveToZenJ1.Text="";
            textBoxPatternAdjectiveToZenJ2.Text="";
            textBoxPatternAdjectiveToZenJ3.Text="";
            textBoxPatternAdjectiveToZenJ4.Text="";
            textBoxPatternAdjectiveToZenJ5.Text="";
            textBoxPatternAdjectiveToZenJ6.Text="";
            textBoxPatternAdjectiveToZenJ7.Text="";

            textBoxPatternAdjectiveToZenM1.Text="";
            textBoxPatternAdjectiveToZenM2.Text="";
            textBoxPatternAdjectiveToZenM3.Text="";
            textBoxPatternAdjectiveToZenM4.Text="";
            textBoxPatternAdjectiveToZenM5.Text="";
            textBoxPatternAdjectiveToZenM6.Text="";
            textBoxPatternAdjectiveToZenM7.Text="";

            textBoxPatternAdjectiveToMuzJ1.Text="";
            textBoxPatternAdjectiveToMuzJ2.Text="";
            textBoxPatternAdjectiveToMuzJ3.Text="";
            textBoxPatternAdjectiveToMuzJ4.Text="";
            textBoxPatternAdjectiveToMuzJ5.Text="";
            textBoxPatternAdjectiveToMuzJ6.Text="";
            textBoxPatternAdjectiveToMuzJ7.Text="";

            textBoxPatternAdjectiveToMuzM1.Text="";
            textBoxPatternAdjectiveToMuzM2.Text="";
            textBoxPatternAdjectiveToMuzM3.Text="";
            textBoxPatternAdjectiveToMuzM4.Text="";
            textBoxPatternAdjectiveToMuzM5.Text="";
            textBoxPatternAdjectiveToMuzM6.Text="";
            textBoxPatternAdjectiveToMuzM7.Text="";

            textBoxPatternAdjectiveToMunJ1.Text="";
            textBoxPatternAdjectiveToMunJ2.Text="";
            textBoxPatternAdjectiveToMunJ3.Text="";
            textBoxPatternAdjectiveToMunJ4.Text="";
            textBoxPatternAdjectiveToMunJ5.Text="";
            textBoxPatternAdjectiveToMunJ6.Text="";
            textBoxPatternAdjectiveToMunJ7.Text="";

            textBoxPatternAdjectiveToMunM1.Text="";
            textBoxPatternAdjectiveToMunM2.Text="";
            textBoxPatternAdjectiveToMunM3.Text="";
            textBoxPatternAdjectiveToMunM4.Text="";
            textBoxPatternAdjectiveToMunM5.Text="";
            textBoxPatternAdjectiveToMunM6.Text="";
            textBoxPatternAdjectiveToMunM7.Text="";
            textBoxPatternAdjectiveToName.Visible=false;
            labelPatternAdjectiveToStr.Visible=false;
            tableLayoutPanelPatternAdjectiveToStr.Visible=false;
            labelPatternAdjectiveToStrFall.Visible=false;
            labelPatternAdjectiveToStrMultiple.Visible=false;
            labelPatternAdjectiveToStrSingle.Visible=false;

            tableLayoutPanelPatternAdjectiveToMuz.Visible=false;
            labelPatternAdjectiveToMuzFall.Visible=false;
            labelPatternAdjectiveToMuzMultiple.Visible=false;
            labelPatternAdjectiveToMuzSingle.Visible=false;
            labelPatternAdjectiveToMuz.Visible=false;
            tableLayoutPanelPatternAdjectiveToMun.Visible=false;
            labelPatternAdjectiveToMunFall.Visible=false;
            labelPatternAdjectiveToMunMultiple.Visible=false;
            labelPatternAdjectiveToMunSingle.Visible=false;
            labelPatternAdjectiveToZen.Visible=false;
            tableLayoutPanelPatternAdjectiveToZen.Visible=false;
            labelPatternAdjectiveToZenFall.Visible=false;
            labelPatternAdjectiveToZenMultiple.Visible=false;
            labelPatternAdjectiveToZenSingle.Visible=false;
            labelPatternAdjectiveToMun.Visible=false;
            labelPatternAdjectiveToName.Visible=false;
        }
        
        void PatternAdjectiveToClear() { 
            listBoxPatternAdjectiveTo.Items.Clear();
            PatternAdjectiveToSetNone();
            itemsPatternAdjectivesToFiltered?.Clear();
            itemsPatternAdjectiveTo?.Clear();
            CurrentPatternAdjectiveTo=null;
        }
        #endregion

        #region Adjective
        void ListBoxAdjective_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentAdjective();
            
            int index=listBoxAdjective.SelectedIndex;
            if (itemsAdjectives.Count==0) {
                SetNoneAdjective();
                return;
            }
            if (index>=itemsAdjectives.Count) 
                index=itemsAdjectives.Count-1;
            if (index<0)
                index=0;
           
            CurrentAdjective=itemsAdjectives[index];
            SetCurrentAdjective();
            SetListBoxAdjective();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void ButtonAdjectiveAdd_Click(object sender, EventArgs e) {
            AddNewItemAdjective();
        }

        void buttonAdjectiveRemove_Click(object sender, EventArgs e) {
            RemoveItemAdjective(CurrentAdjective);
            TextBoxAdjectiveFilter_TextChanged(null, new EventArgs());
        }
        
        void TextBoxAdjectiveFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentAdjective();

            // Získej aktuální prvek
            ItemAdjective selectedId=null;
            if (listBoxAdjective.SelectedIndex!=-1) {
                selectedId=itemsAdjectivesFiltered[listBoxAdjective.SelectedIndex];
            }
            
            AdjectiveRefreshFilteredList();

            listBoxAdjective.Items.Clear();
            for (int i=0; i<itemsAdjectivesFiltered.Count; i++) {
                ItemAdjective item = itemsAdjectivesFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxAdjective.Items.Add(textToAdd);
            }

            //SetListBoxAdjective();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsAdjectivesFiltered.Count; i++){
                    if (itemsAdjectivesFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxAdjective.SelectedIndex=-1;
                    CurrentAdjective=null;
                } else listBoxAdjective.SelectedIndex=outIndex;
            } else {
                listBoxAdjective.SelectedIndex=-1;
                CurrentAdjective=null;
            }
            SetCurrentAdjective();
        }
            
        void RemoveCurrentAdjective(object sender, EventArgs e) {
            itemsAdjectives.Remove(CurrentAdjective);
        }

        void SetListBoxAdjective() { 
            string filter=textBoxAdjectiveFilter.Text;
            //bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxAdjective.SelectedIndex;
            listBoxAdjective.Items.Clear();
            for (int i=0; i<itemsAdjectivesFiltered.Count; i++) {
                ItemAdjective item = itemsAdjectivesFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxAdjective.Items.Add(textToAdd);
            }

            if (index>=listBoxAdjective.Items.Count)index=listBoxAdjective.Items.Count-1;
            listBoxAdjective.SelectedIndex=index;
        }

        void AdjectiveRefreshFilteredList() {
            if (itemsAdjectivesFiltered==null) itemsAdjectivesFiltered=new List<ItemAdjective>();
            itemsAdjectivesFiltered.Clear();
            string filter=textBoxAdjectiveFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsAdjectives.Count; i++) {
                    ItemAdjective item = itemsAdjectives[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsAdjectivesFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsAdjectives.Count; i++) {
                    ItemAdjective item = itemsAdjectives[i];
                    itemsAdjectivesFiltered.Add(item);
                }
            }
        }
               
        void AddNewItemAdjective() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentAdjective();

            var newItem=new ItemAdjective();
           // newItem.ID=itemsAdjectives.Count;
            itemsAdjectives.Add(newItem);
            CurrentAdjective=newItem;
            AdjectiveRefreshFilteredList();
            SetListBoxAdjective(); 
            ListBoxSetCurrentAdjective();
            SetCurrentAdjective(); 
            
            doingJob=false;
        }
   
        void RemoveItemAdjective(ItemAdjective item) { 
            Edited=true;
            ChangeCaptionText();
            itemsAdjectives.Remove(item);
            AdjectiveRefreshFilteredList();
            SetListBoxAdjective();
            SetCurrentAdjective();
        } 
           
        void SetCurrentAdjective(){
            if (itemsAdjectivesFiltered.Count==0) {
                SetNoneAdjective();
                return;
            }

            int index=listBoxAdjective.SelectedIndex;
            if (index>=itemsAdjectivesFiltered.Count) index=itemsAdjectivesFiltered.Count-1;
            if (index<0) index=0;
            CurrentAdjective=itemsAdjectivesFiltered[index];
            
            textBoxAdjectiveFrom.Visible=true;
            textBoxAdjectiveTo.Visible=true;
            labelAdjectiveFrom.Visible=true;
            labelAdjectiveTo.Visible=true; 

            textBoxAdjectiveFrom.Text=CurrentAdjective.From;
            textBoxAdjectiveTo.Text=CurrentAdjective.To;

            comboBoxAdjectiveInputPatternFrom.Text=CurrentAdjective.PatternFrom;

            comboBoxAdjectiveInputPatternFrom.Items.Clear();
            comboBoxAdjectiveInputPatternTo.Items.Clear();
            foreach (ItemPatternAdjective x in itemsPatternAdjectiveFrom) {
                comboBoxAdjectiveInputPatternFrom.Items.Add(x.Name);
                comboBoxAdjectiveInputPatternTo.Items.Add(x.Name);
            }
            
            comboBoxAdjectiveInputPatternTo.Text=CurrentAdjective.PatternTo;
      
            comboBoxAdjectiveInputPatternFrom.Visible=true; 
            comboBoxAdjectiveInputPatternTo.Visible=true; 

            labelAdjectiveInputPatternFrom.Visible=true; 
            labelAdjectiveInputPatternTo.Visible=true; 

            labelAdjectiveShowFrom.Visible=true;
            labelAdjectiveShowTo.Visible=true;
        }
         
        void ListBoxSetCurrentAdjective() {
            for (int indexCur=0; indexCur<itemsAdjectivesFiltered.Count; indexCur++) {
                if (itemsAdjectives[indexCur]==CurrentAdjective) { 
                    int indexList=listBoxAdjective.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxAdjective.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void SaveCurrentAdjective() {
            if (CurrentAdjective==null) return;
                     
            CurrentAdjective.From=textBoxAdjectiveFrom.Text;
            CurrentAdjective.To=textBoxAdjectiveTo.Text;
            
            CurrentAdjective.PatternFrom=comboBoxAdjectiveInputPatternFrom.Text;
            CurrentAdjective.PatternTo=comboBoxAdjectiveInputPatternTo.Text;
        } 
              
        void SetNoneAdjective(){ 
            textBoxAdjectiveFrom.Text="";
            textBoxAdjectiveTo.Text="";

            comboBoxAdjectiveInputPatternFrom.Text="";
            comboBoxAdjectiveInputPatternTo.Text="";

            textBoxAdjectiveFrom.Visible=false;
            textBoxAdjectiveTo.Visible=false;
            labelAdjectiveFrom.Visible=false;
            labelAdjectiveTo.Visible=false;
            comboBoxAdjectiveInputPatternTo.Visible=false;
            comboBoxAdjectiveInputPatternFrom.Visible=false;
            labelAdjectiveShowFrom.Visible=false;
            labelAdjectiveShowTo.Visible=false;
            labelAdjectiveInputPatternFrom.Visible=false; 
            labelAdjectiveInputPatternTo.Visible=false; 
        }

        void ClearAdjective(){ 
            listBoxAdjective.Items.Clear();
            SetNoneAdjective();
            itemsAdjectivesFiltered?.Clear();
            itemsAdjectives?.Clear();
            CurrentAdjective=null;
        }
        #endregion
 
        #region PronounPatternFrom
        void PatternPronounFromListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternPronounFromSaveCurrent();
            
            int index=listBoxPatternPronounFrom.SelectedIndex;
            if (itemsPatternPronounFrom.Count==0) {
                PatternPronounFromSetNone();
                return;
            }
            if (index>=itemsPatternPronounFrom.Count) 
                index=itemsPatternPronounFrom.Count-1;
            if (index<0)
                index=0;
           
            CurrentPatternPronounFrom=itemsPatternPronounFrom[index];
            PatternPronounFromSetCurrent();
            PatternPronounFromSetListBox();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void PatternPronounFromButtonAdd_Click(object sender, EventArgs e) {
            PatternPronounFromAddNewItem();
        }

        void PatternPronounFromButtonRemove_Click(object sender, EventArgs e) {
            PatternPronounFromRemoveItem(CurrentPatternPronounFrom);
            PatternPronounFromTextBoxFilter_TextChanged(null, new EventArgs());
        }
        
        void PatternPronounFromTextBoxFilter_TextChanged(object sender, EventArgs e) {
            PatternPronounFromSaveCurrent();

            // Získej aktuální prvek
            ItemPatternPronoun selectedId=null;
            if (listBoxPatternPronounFrom.SelectedIndex!=-1) {
                selectedId=itemsPatternPronounsFromFiltered[listBoxPatternPronounFrom.SelectedIndex];
            }
            
            PatternPronounFromRefreshFilteredList();

            listBoxPatternPronounFrom.Items.Clear();
            for (int i=0; i<itemsPatternPronounsFromFiltered.Count; i++) {
                ItemPatternPronoun item = itemsPatternPronounsFromFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternPronounFrom.Items.Add(textToAdd);
            }

            //SetListBoxPronoun();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsPatternPronounsFromFiltered.Count; i++){
                    if (itemsPatternPronounsFromFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxPatternPronounFrom.SelectedIndex=-1;
                    CurrentPatternPronounFrom=null;
                } else listBoxPatternPronounFrom.SelectedIndex=outIndex;
            } else {
                listBoxPatternPronounFrom.SelectedIndex=-1;
                CurrentPatternPronounFrom=null;
            }
            PatternPronounFromSetCurrent();
        }
            
        void PatternPronounFromRemoveCurrent(object sender, EventArgs e) {
            itemsPatternPronounFrom.Remove(CurrentPatternPronounFrom);
        }

        void PatternPronounFromSetListBox() { 
            string filter=textBoxPatternPronounFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxPatternPronounFrom.SelectedIndex;
            listBoxPatternPronounFrom.Items.Clear();
            for (int i=0; i<itemsPatternPronounsFromFiltered.Count; i++) {
                ItemPatternPronoun item = itemsPatternPronounsFromFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternPronounFrom.Items.Add(textToAdd);
            }

            if (index>=listBoxPatternPronounFrom.Items.Count)index=listBoxPatternPronounFrom.Items.Count-1;
            listBoxPatternPronounFrom.SelectedIndex=index;
        }

        void PatternPronounFromRefreshFilteredList() {
            if (itemsPatternPronounsFromFiltered==null) itemsPatternPronounsFromFiltered=new List<ItemPatternPronoun>();
            itemsPatternPronounsFromFiltered.Clear();
            string filter=textBoxPatternPronounFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsPatternPronounFrom.Count; i++) {
                    ItemPatternPronoun item = itemsPatternPronounFrom[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsPatternPronounsFromFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsPatternPronounFrom.Count; i++) {
                    ItemPatternPronoun item = itemsPatternPronounFrom[i];
                    itemsPatternPronounsFromFiltered.Add(item);
                }
            }
        }
               
        void PatternPronounFromAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternPronounFromSaveCurrent();

            var newItem=new ItemPatternPronoun();
           // newItem.ID=itemsPronouns.Count;
            itemsPatternPronounFrom.Add(newItem);
            CurrentPatternPronounFrom=newItem;
            PatternPronounFromRefreshFilteredList();
            PatternPronounFromSetListBox(); 
            PatternPronounFromListBoxSetCurrent();
            PatternPronounFromSetCurrent(); 
            
            doingJob=false;
        }
   
        void PatternPronounFromRemoveItem(ItemPatternPronoun item) { 
            Edited=true;
            ChangeCaptionText();
            itemsPatternPronounFrom.Remove(item);
            PatternPronounFromRefreshFilteredList();
            PatternPronounFromSetListBox();
            PatternPronounFromSetCurrent();
        } 
           
        void PatternPronounFromSetCurrent(){
            if (itemsPatternPronounsFromFiltered.Count==0) {
                PatternPronounFromSetNone();
                return;
            }

            int index=listBoxPatternPronounFrom.SelectedIndex;
            if (index>=itemsPatternPronounsFromFiltered.Count) index=itemsPatternPronounsFromFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternPronounFrom=itemsPatternPronounsFromFiltered[index];
            
            textBoxPatternPronounName.Text=CurrentPatternPronounFrom.Name;
            comboBoxPatternPronounType.SelectedIndex=(int)CurrentPatternPronounFrom.Type;
          //  comboBoxPatternPronounGender.SelectedIndex=(int)CurrentPatternPronoun.Gender;

            //textBoxPatternPronounMuzJ1.Text=CurrentPatternPronoun.Shapes[0];
            //textBoxPatternPronounMuzJ2.Text=CurrentPatternPronoun.Shapes[1];
            //textBoxPatternPronounMuzJ3.Text=CurrentPatternPronoun.Shapes[2];
            //textBoxPatternPronounMuzJ4.Text=CurrentPatternPronoun.Shapes[3];
            //textBoxPatternPronounMuzJ5.Text=CurrentPatternPronoun.Shapes[4];
            //textBoxPatternPronounMuzJ6.Text=CurrentPatternPronoun.Shapes[5];
            //textBoxPatternPronounMuzJ7.Text=CurrentPatternPronoun.Shapes[6];

            //textBoxPatternPronounMuzM1.Text=CurrentPatternPronoun.Shapes[7];
            //textBoxPatternPronounMuzM2.Text=CurrentPatternPronoun.Shapes[8];
            //textBoxPatternPronounMuzM3.Text=CurrentPatternPronoun.Shapes[9];
            //textBoxPatternPronounMuzM4.Text=CurrentPatternPronoun.Shapes[10];
            //textBoxPatternPronounMuzM5.Text=CurrentPatternPronoun.Shapes[11];
            //textBoxPatternPronounMuzM6.Text=CurrentPatternPronoun.Shapes[12];
            //textBoxPatternPronounMuzM7.Text=CurrentPatternPronoun.Shapes[13];

             switch (CurrentPatternPronounFrom.Type) { 
                case PronounType.Unknown:
                    break;
                    
                case PronounType.NoDeklination:
                     textBoxPatternPronounFromMuzJ1.Text=CurrentPatternPronounFrom.Shapes[0];
                    break;

                case PronounType.DeklinationOnlySingle:
                    textBoxPatternPronounFromMuzJ1.Text=CurrentPatternPronounFrom.Shapes[0];
                    textBoxPatternPronounFromMuzJ2.Text=CurrentPatternPronounFrom.Shapes[1];
                    textBoxPatternPronounFromMuzJ3.Text=CurrentPatternPronounFrom.Shapes[2];
                    textBoxPatternPronounFromMuzJ4.Text=CurrentPatternPronounFrom.Shapes[3];
                    textBoxPatternPronounFromMuzJ5.Text=CurrentPatternPronounFrom.Shapes[4];
                    textBoxPatternPronounFromMuzJ6.Text=CurrentPatternPronounFrom.Shapes[5];
                    textBoxPatternPronounFromMuzJ7.Text=CurrentPatternPronounFrom.Shapes[6];
                    break;
                    
                case PronounType.Deklination:
                    textBoxPatternPronounFromMuzJ1.Text=CurrentPatternPronounFrom.Shapes[0];
                    textBoxPatternPronounFromMuzJ2.Text=CurrentPatternPronounFrom.Shapes[1];
                    textBoxPatternPronounFromMuzJ3.Text=CurrentPatternPronounFrom.Shapes[2];
                    textBoxPatternPronounFromMuzJ4.Text=CurrentPatternPronounFrom.Shapes[3];
                    textBoxPatternPronounFromMuzJ5.Text=CurrentPatternPronounFrom.Shapes[4];
                    textBoxPatternPronounFromMuzJ6.Text=CurrentPatternPronounFrom.Shapes[5];
                    textBoxPatternPronounFromMuzJ7.Text=CurrentPatternPronounFrom.Shapes[6];

                    textBoxPatternPronounFromMuzM1.Text=CurrentPatternPronounFrom.Shapes[7];
                    textBoxPatternPronounFromMuzM2.Text=CurrentPatternPronounFrom.Shapes[8];
                    textBoxPatternPronounFromMuzM3.Text=CurrentPatternPronounFrom.Shapes[9];
                    textBoxPatternPronounFromMuzM4.Text=CurrentPatternPronounFrom.Shapes[10];
                    textBoxPatternPronounFromMuzM5.Text=CurrentPatternPronounFrom.Shapes[11];
                    textBoxPatternPronounFromMuzM6.Text=CurrentPatternPronounFrom.Shapes[12];
                    textBoxPatternPronounFromMuzM7.Text=CurrentPatternPronounFrom.Shapes[13];
                    break;
                    
                case PronounType.DeklinationWithGender:
                    textBoxPatternPronounFromMuzJ1.Text=CurrentPatternPronounFrom.Shapes[0] ;
                    textBoxPatternPronounFromMuzJ2.Text=CurrentPatternPronounFrom.Shapes[1] ;
                    textBoxPatternPronounFromMuzJ3.Text=CurrentPatternPronounFrom.Shapes[2] ;
                    textBoxPatternPronounFromMuzJ4.Text=CurrentPatternPronounFrom.Shapes[3] ;
                    textBoxPatternPronounFromMuzJ5.Text=CurrentPatternPronounFrom.Shapes[4] ;
                    textBoxPatternPronounFromMuzJ6.Text=CurrentPatternPronounFrom.Shapes[5] ;
                    textBoxPatternPronounFromMuzJ7.Text=CurrentPatternPronounFrom.Shapes[6] ;
                    textBoxPatternPronounFromMuzM1.Text=CurrentPatternPronounFrom.Shapes[7] ;
                    textBoxPatternPronounFromMuzM2.Text=CurrentPatternPronounFrom.Shapes[8] ;
                    textBoxPatternPronounFromMuzM3.Text=CurrentPatternPronounFrom.Shapes[9] ;
                    textBoxPatternPronounFromMuzM4.Text=CurrentPatternPronounFrom.Shapes[10];
                    textBoxPatternPronounFromMuzM5.Text=CurrentPatternPronounFrom.Shapes[11];
                    textBoxPatternPronounFromMuzM6.Text=CurrentPatternPronounFrom.Shapes[12];
                    textBoxPatternPronounFromMuzM7.Text=CurrentPatternPronounFrom.Shapes[13];

                    textBoxPatternPronounFromMunJ1.Text=CurrentPatternPronounFrom.Shapes[14];
                    textBoxPatternPronounFromMunJ2.Text=CurrentPatternPronounFrom.Shapes[15];
                    textBoxPatternPronounMunJ3.Text=CurrentPatternPronounFrom.Shapes[16];
                    textBoxPatternPronounFromMunJ4.Text=CurrentPatternPronounFrom.Shapes[17];
                    textBoxPatternPronounFromMunJ5.Text=CurrentPatternPronounFrom.Shapes[18];
                    textBoxPatternPronounFromMunJ6.Text=CurrentPatternPronounFrom.Shapes[19];
                    textBoxPatternPronounFromMunJ7.Text=CurrentPatternPronounFrom.Shapes[20];
                    textBoxPatternPronounMunM1.Text=CurrentPatternPronounFrom.Shapes[21];
                    textBoxPatternPronounMunM2.Text=CurrentPatternPronounFrom.Shapes[22];
                    textBoxPatternPronounMunM3.Text=CurrentPatternPronounFrom.Shapes[23];
                    textBoxPatternPronounMunM4.Text=CurrentPatternPronounFrom.Shapes[24];
                    textBoxPatternPronounMunM5.Text=CurrentPatternPronounFrom.Shapes[25];
                    textBoxPatternPronounMunM6.Text=CurrentPatternPronounFrom.Shapes[26];
                    textBoxPatternPronounFromMunM7.Text=CurrentPatternPronounFrom.Shapes[27];

                    textBoxPatternPronounZenJ1.Text=CurrentPatternPronounFrom.Shapes[28];
                    textBoxPatternPronounZenJ2.Text=CurrentPatternPronounFrom.Shapes[29];
                    textBoxPatternPronounZenJ3.Text=CurrentPatternPronounFrom.Shapes[30];
                    textBoxPatternPronounZenJ4.Text=CurrentPatternPronounFrom.Shapes[31];
                    textBoxPatternPronounZenJ5.Text=CurrentPatternPronounFrom.Shapes[32];
                    textBoxPatternPronounZenJ6.Text=CurrentPatternPronounFrom.Shapes[33];
                    textBoxPatternPronounZenJ7.Text=CurrentPatternPronounFrom.Shapes[34];
                    textBoxPatternPronounZenM1.Text=CurrentPatternPronounFrom.Shapes[35];
                    textBoxPatternPronounZenM2.Text=CurrentPatternPronounFrom.Shapes[36];
                    textBoxPatternPronounZenM3.Text=CurrentPatternPronounFrom.Shapes[37];
                    textBoxPatternPronounZenM4.Text=CurrentPatternPronounFrom.Shapes[38];
                    textBoxPatternPronounZenM5.Text=CurrentPatternPronounFrom.Shapes[39];
                    textBoxPatternPronounZenM6.Text=CurrentPatternPronounFrom.Shapes[40];
                    textBoxPatternPronounZenM7.Text=CurrentPatternPronounFrom.Shapes[41];

                    textBoxPatternPronounStrJ1.Text=CurrentPatternPronounFrom.Shapes[42];
                    textBoxPatternPronounStrJ2.Text=CurrentPatternPronounFrom.Shapes[43];
                    textBoxPatternPronounStrJ3.Text=CurrentPatternPronounFrom.Shapes[44];
                    textBoxPatternPronounStrJ4.Text=CurrentPatternPronounFrom.Shapes[45];
                    textBoxPatternPronounStrJ5.Text=CurrentPatternPronounFrom.Shapes[46];
                    textBoxPatternPronounStrJ6.Text=CurrentPatternPronounFrom.Shapes[47];
                    textBoxPatternPronounStrJ7.Text=CurrentPatternPronounFrom.Shapes[48];
                    textBoxPatternPronounStrM1.Text=CurrentPatternPronounFrom.Shapes[49];
                    textBoxPatternPronounStrM2.Text=CurrentPatternPronounFrom.Shapes[50];
                    textBoxPatternPronounStrM3.Text=CurrentPatternPronounFrom.Shapes[51];
                    textBoxPatternPronounStrM4.Text=CurrentPatternPronounFrom.Shapes[52];
                    textBoxPatternPronounStrM5.Text=CurrentPatternPronounFrom.Shapes[53];
                    textBoxPatternPronounStrM6.Text=CurrentPatternPronounFrom.Shapes[54];
                    textBoxPatternPronounStrM7.Text=CurrentPatternPronounFrom.Shapes[55];
                    break;
            }

            labelPatternPronounName.Visible=true;
            textBoxPatternPronounName.Visible=true;
            labelPatternPronounType.Visible=true;
            comboBoxPatternPronounType.Visible=true;

            labelPatternPronounMuzFall.Visible=true;
            labelPatternPronounMuzSingle.Visible=true;
            labelPatternPronounMuzMultiple.Visible=true;
            tableLayoutPanelPatternPronounMuz.Visible=true; 
            
           //abelPatternPronounInfo.Visible=true;
            ChangeTypePatternPronoun(CurrentPatternPronounFrom?.Type);
        }
         
        void PatternPronounFromListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternPronounsFromFiltered.Count; indexCur++) {
                if (itemsPatternPronounFrom[indexCur]==CurrentPatternPronounFrom) { 
                    int indexList=listBoxPatternPronounFrom.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxPatternPronounFrom.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void PatternPronounFromSaveCurrent() {
            if (CurrentPatternPronounFrom==null) return;
            comboBoxPatternPronounType.SelectedIndex=(int) CurrentPatternPronounFrom.Type;
            CurrentPatternPronounFrom.Name=textBoxPatternPronounName.Text;
            switch (CurrentPatternPronounFrom.Type) { 
                case PronounType.Unknown:
                    break;

                case PronounType.NoDeklination:
                    CurrentPatternPronounFrom.Shapes[0]=textBoxPatternPronounFromMuzJ1.Text;
                    break;
                    
                case PronounType.DeklinationOnlySingle:
                    CurrentPatternPronounFrom.Shapes[0]=textBoxPatternPronounFromMuzJ1.Text;
                    CurrentPatternPronounFrom.Shapes[1]=textBoxPatternPronounFromMuzJ2.Text;
                    CurrentPatternPronounFrom.Shapes[2]=textBoxPatternPronounFromMuzJ3.Text;
                    CurrentPatternPronounFrom.Shapes[3]=textBoxPatternPronounFromMuzJ4.Text;
                    CurrentPatternPronounFrom.Shapes[4]=textBoxPatternPronounFromMuzJ5.Text;
                    CurrentPatternPronounFrom.Shapes[5]=textBoxPatternPronounFromMuzJ6.Text;
                    CurrentPatternPronounFrom.Shapes[6]=textBoxPatternPronounFromMuzJ7.Text;
                    break;
                    
                case PronounType.Deklination:
                    CurrentPatternPronounFrom.Shapes[0]=textBoxPatternPronounFromMuzJ1.Text;
                    CurrentPatternPronounFrom.Shapes[1]=textBoxPatternPronounFromMuzJ2.Text;
                    CurrentPatternPronounFrom.Shapes[2]=textBoxPatternPronounFromMuzJ3.Text;
                    CurrentPatternPronounFrom.Shapes[3]=textBoxPatternPronounFromMuzJ4.Text;
                    CurrentPatternPronounFrom.Shapes[4]=textBoxPatternPronounFromMuzJ5.Text;
                    CurrentPatternPronounFrom.Shapes[5]=textBoxPatternPronounFromMuzJ6.Text;
                    CurrentPatternPronounFrom.Shapes[6]=textBoxPatternPronounFromMuzJ7.Text;

                    CurrentPatternPronounFrom.Shapes[7]=textBoxPatternPronounFromMuzM1.Text;
                    CurrentPatternPronounFrom.Shapes[8]=textBoxPatternPronounFromMuzM2.Text;
                    CurrentPatternPronounFrom.Shapes[9]=textBoxPatternPronounFromMuzM3.Text;
                    CurrentPatternPronounFrom.Shapes[10]=textBoxPatternPronounFromMuzM4.Text;
                    CurrentPatternPronounFrom.Shapes[11]=textBoxPatternPronounFromMuzM5.Text;
                    CurrentPatternPronounFrom.Shapes[12]=textBoxPatternPronounFromMuzM6.Text;
                    CurrentPatternPronounFrom.Shapes[13]=textBoxPatternPronounFromMuzM7.Text;
                    break;
                    
                case PronounType.DeklinationWithGender:
                    CurrentPatternPronounFrom.Shapes[0]=textBoxPatternPronounFromMuzJ1.Text;
                    CurrentPatternPronounFrom.Shapes[1]=textBoxPatternPronounFromMuzJ2.Text;
                    CurrentPatternPronounFrom.Shapes[2]=textBoxPatternPronounFromMuzJ3.Text;
                    CurrentPatternPronounFrom.Shapes[3]=textBoxPatternPronounFromMuzJ4.Text;
                    CurrentPatternPronounFrom.Shapes[4]=textBoxPatternPronounFromMuzJ5.Text;
                    CurrentPatternPronounFrom.Shapes[5]=textBoxPatternPronounFromMuzJ6.Text;
                    CurrentPatternPronounFrom.Shapes[6]=textBoxPatternPronounFromMuzJ7.Text;
                    CurrentPatternPronounFrom.Shapes[7]=textBoxPatternPronounFromMuzM1.Text;
                    CurrentPatternPronounFrom.Shapes[8]=textBoxPatternPronounFromMuzM2.Text;
                    CurrentPatternPronounFrom.Shapes[9]=textBoxPatternPronounFromMuzM3.Text;
                    CurrentPatternPronounFrom.Shapes[10]=textBoxPatternPronounFromMuzM4.Text;
                    CurrentPatternPronounFrom.Shapes[11]=textBoxPatternPronounFromMuzM5.Text;
                    CurrentPatternPronounFrom.Shapes[12]=textBoxPatternPronounFromMuzM6.Text;
                    CurrentPatternPronounFrom.Shapes[13]=textBoxPatternPronounFromMuzM7.Text;

                    CurrentPatternPronounFrom.Shapes[14]=textBoxPatternPronounFromMunJ1.Text;
                    CurrentPatternPronounFrom.Shapes[15]=textBoxPatternPronounFromMunJ2.Text;
                    CurrentPatternPronounFrom.Shapes[16]=textBoxPatternPronounMunJ3.Text;
                    CurrentPatternPronounFrom.Shapes[17]=textBoxPatternPronounFromMunJ4.Text;
                    CurrentPatternPronounFrom.Shapes[18]=textBoxPatternPronounFromMunJ5.Text;
                    CurrentPatternPronounFrom.Shapes[19]=textBoxPatternPronounFromMunJ6.Text;
                    CurrentPatternPronounFrom.Shapes[20]=textBoxPatternPronounFromMunJ7.Text;
                    CurrentPatternPronounFrom.Shapes[21]=textBoxPatternPronounMunM1.Text;
                    CurrentPatternPronounFrom.Shapes[22]=textBoxPatternPronounMunM2.Text;
                    CurrentPatternPronounFrom.Shapes[23]=textBoxPatternPronounMunM3.Text;
                    CurrentPatternPronounFrom.Shapes[24]=textBoxPatternPronounMunM4.Text;
                    CurrentPatternPronounFrom.Shapes[25]=textBoxPatternPronounMunM5.Text;
                    CurrentPatternPronounFrom.Shapes[26]=textBoxPatternPronounMunM6.Text;
                    CurrentPatternPronounFrom.Shapes[27]=textBoxPatternPronounFromMunM7.Text;

                    CurrentPatternPronounFrom.Shapes[28]=textBoxPatternPronounZenJ1.Text;
                    CurrentPatternPronounFrom.Shapes[29]=textBoxPatternPronounZenJ2.Text;
                    CurrentPatternPronounFrom.Shapes[30]=textBoxPatternPronounZenJ3.Text;
                    CurrentPatternPronounFrom.Shapes[31]=textBoxPatternPronounZenJ4.Text;
                    CurrentPatternPronounFrom.Shapes[32]=textBoxPatternPronounZenJ5.Text;
                    CurrentPatternPronounFrom.Shapes[33]=textBoxPatternPronounZenJ6.Text;
                    CurrentPatternPronounFrom.Shapes[34]=textBoxPatternPronounZenJ7.Text;
                    CurrentPatternPronounFrom.Shapes[35]=textBoxPatternPronounZenM1.Text;
                    CurrentPatternPronounFrom.Shapes[36]=textBoxPatternPronounZenM2.Text;
                    CurrentPatternPronounFrom.Shapes[37]=textBoxPatternPronounZenM3.Text;
                    CurrentPatternPronounFrom.Shapes[38]=textBoxPatternPronounZenM4.Text;
                    CurrentPatternPronounFrom.Shapes[39]=textBoxPatternPronounZenM5.Text;
                    CurrentPatternPronounFrom.Shapes[40]=textBoxPatternPronounZenM6.Text;
                    CurrentPatternPronounFrom.Shapes[41]=textBoxPatternPronounZenM7.Text;

                    CurrentPatternPronounFrom.Shapes[42]=textBoxPatternPronounStrJ1.Text;
                    CurrentPatternPronounFrom.Shapes[43]=textBoxPatternPronounStrJ2.Text;
                    CurrentPatternPronounFrom.Shapes[44]=textBoxPatternPronounStrJ3.Text;
                    CurrentPatternPronounFrom.Shapes[45]=textBoxPatternPronounStrJ4.Text;
                    CurrentPatternPronounFrom.Shapes[46]=textBoxPatternPronounStrJ5.Text;
                    CurrentPatternPronounFrom.Shapes[47]=textBoxPatternPronounStrJ6.Text;
                    CurrentPatternPronounFrom.Shapes[48]=textBoxPatternPronounStrJ7.Text;
                    CurrentPatternPronounFrom.Shapes[49]=textBoxPatternPronounStrM1.Text;
                    CurrentPatternPronounFrom.Shapes[50]=textBoxPatternPronounStrM2.Text;
                    CurrentPatternPronounFrom.Shapes[51]=textBoxPatternPronounStrM3.Text;
                    CurrentPatternPronounFrom.Shapes[52]=textBoxPatternPronounStrM4.Text;
                    CurrentPatternPronounFrom.Shapes[53]=textBoxPatternPronounStrM5.Text;
                    CurrentPatternPronounFrom.Shapes[54]=textBoxPatternPronounStrM6.Text;
                    CurrentPatternPronounFrom.Shapes[55]=textBoxPatternPronounStrM7.Text;
                    break;
            }
        } 
              
        void PatternPronounFromSetNone(){ 
            textBoxPatternPronounName.Text="";

            textBoxPatternPronounFromMuzJ1.Text="";
            textBoxPatternPronounFromMuzJ2.Text="";
            textBoxPatternPronounFromMuzJ3.Text="";
            textBoxPatternPronounFromMuzJ4.Text="";
            textBoxPatternPronounFromMuzJ5.Text="";
            textBoxPatternPronounFromMuzJ6.Text="";
            textBoxPatternPronounFromMuzJ7.Text="";

            textBoxPatternPronounFromMuzM1.Text="";
            textBoxPatternPronounFromMuzM2.Text="";
            textBoxPatternPronounFromMuzM3.Text="";
            textBoxPatternPronounFromMuzM4.Text="";
            textBoxPatternPronounFromMuzM5.Text="";
            textBoxPatternPronounFromMuzM6.Text="";
            textBoxPatternPronounFromMuzM7.Text="";

            
            textBoxPatternPronounZenJ1.Text="";
            textBoxPatternPronounZenJ2.Text="";
            textBoxPatternPronounZenJ3.Text="";
            textBoxPatternPronounZenJ4.Text="";
            textBoxPatternPronounZenJ5.Text="";
            textBoxPatternPronounZenJ6.Text="";
            textBoxPatternPronounZenJ7.Text="";

            textBoxPatternPronounZenM1.Text="";
            textBoxPatternPronounZenM2.Text="";
            textBoxPatternPronounZenM3.Text="";
            textBoxPatternPronounZenM4.Text="";
            textBoxPatternPronounZenM5.Text="";
            textBoxPatternPronounZenM6.Text="";
            textBoxPatternPronounZenM7.Text="";

            textBoxPatternPronounFromMunJ1.Text="";
            textBoxPatternPronounFromMunJ2.Text="";
            textBoxPatternPronounMunJ3.Text="";
            textBoxPatternPronounFromMunJ4.Text="";
            textBoxPatternPronounFromMunJ5.Text="";
            textBoxPatternPronounFromMunJ6.Text="";
            textBoxPatternPronounFromMunJ7.Text="";

            textBoxPatternPronounMunM1.Text="";
            textBoxPatternPronounMunM2.Text="";
            textBoxPatternPronounMunM3.Text="";
            textBoxPatternPronounMunM4.Text="";
            textBoxPatternPronounMunM5.Text="";
            textBoxPatternPronounMunM6.Text="";
            textBoxPatternPronounFromMunM7.Text="";

            textBoxPatternPronounStrJ1.Text="";
            textBoxPatternPronounStrJ2.Text="";
            textBoxPatternPronounStrJ3.Text="";
            textBoxPatternPronounStrJ4.Text="";
            textBoxPatternPronounStrJ5.Text="";
            textBoxPatternPronounStrJ6.Text="";
            textBoxPatternPronounStrJ7.Text="";

            textBoxPatternPronounStrM1.Text="";
            textBoxPatternPronounStrM2.Text="";
            textBoxPatternPronounStrM3.Text="";
            textBoxPatternPronounStrM4.Text="";
            textBoxPatternPronounStrM5.Text="";
            textBoxPatternPronounStrM6.Text="";
            textBoxPatternPronounStrM7.Text="";

            labelPatternPronounName.Visible=false;
            textBoxPatternPronounName.Visible=false;
            labelPatternPronounType.Visible=false;
            comboBoxPatternPronounType.Visible=false;

            labelPatternPronounMuzFall.Visible=false;
            labelPatternPronounMuzSingle.Visible=false;
            labelPatternPronounMuzMultiple.Visible=false;
            tableLayoutPanelPatternPronounMuz.Visible=false;
            //labelPatternPronounInfo.Visible=false;
           // ChangeTypePatternPronoun(CurrentPatternPronoun?.Type);
        }
                
        void PatternPronounFromClear() { 
            listBoxPatternPronounFrom.Items.Clear();
            PatternPronounFromSetNone();
            itemsPatternPronounsFromFiltered?.Clear();
            itemsPatternPronounFrom?.Clear();
            CurrentPatternPronounFrom=null;
        }
        #endregion
        
        #region PronounPatternTo
        void PatternPronounToListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternPronounToSaveCurrent();
            
            int index=listBoxPatternPronounTo.SelectedIndex;
            if (itemsPatternPronounTo.Count==0) {
                PatternPronounToSetNone();
                return;
            }
            if (index>=itemsPatternPronounTo.Count) 
                index=itemsPatternPronounTo.Count-1;
            if (index<0)
                index=0;
           
            CurrentPatternPronounTo=itemsPatternPronounTo[index];
            PatternPronounToSetCurrent();
            PatternPronounToSetListBox();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void PatternPronounToButtonAdd_Click(object sender, EventArgs e) {
            PatternPronounToAddNewItem();
        }

        void PatternPronounToButtonRemove_Click(object sender, EventArgs e) {
            PatternPronounToRemoveItem(CurrentPatternPronounTo);
            PatternPronounToTextBoxFilter_TextChanged(null, new EventArgs());
        }
        
        void PatternPronounToTextBoxFilter_TextChanged(object sender, EventArgs e) {
            PatternPronounToSaveCurrent();

            // Získej aktuální prvek
            ItemPatternPronoun selectedId=null;
            if (listBoxPatternPronounTo.SelectedIndex!=-1) {
                selectedId=itemsPatternPronounsToFiltered[listBoxPatternPronounTo.SelectedIndex];
            }
            
            PatternPronounToRefreshFilteredList();

            listBoxPatternPronounTo.Items.Clear();
            for (int i=0; i<itemsPatternPronounsToFiltered.Count; i++) {
                ItemPatternPronoun item = itemsPatternPronounsToFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternPronounTo.Items.Add(textToAdd);
            }

            //SetListBoxPronoun();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsPatternPronounsToFiltered.Count; i++){
                    if (itemsPatternPronounsToFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxPatternPronounTo.SelectedIndex=-1;
                    CurrentPatternPronounTo=null;
                } else listBoxPatternPronounTo.SelectedIndex=outIndex;
            } else {
                listBoxPatternPronounTo.SelectedIndex=-1;
                CurrentPatternPronounTo=null;
            }
            PatternPronounToSetCurrent();
        }
            
        void PatternPronounToRemoveCurrent(object sender, EventArgs e) {
            itemsPatternPronounTo.Remove(CurrentPatternPronounTo);
        }

        void PatternPronounToSetListBox() { 
            string filter=textBoxPatternPronounFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxPatternPronounTo.SelectedIndex;
            listBoxPatternPronounTo.Items.Clear();
            for (int i=0; i<itemsPatternPronounsToFiltered.Count; i++) {
                ItemPatternPronoun item = itemsPatternPronounsToFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPatternPronounTo.Items.Add(textToAdd);
            }

            if (index>=listBoxPatternPronounTo.Items.Count)index=listBoxPatternPronounTo.Items.Count-1;
            listBoxPatternPronounTo.SelectedIndex=index;
        }

        void PatternPronounToRefreshFilteredList() {
            if (itemsPatternPronounsToFiltered==null) itemsPatternPronounsToFiltered=new List<ItemPatternPronoun>();
            itemsPatternPronounsToFiltered.Clear();
            string filter=textBoxPatternPronounFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsPatternPronounTo.Count; i++) {
                    ItemPatternPronoun item = itemsPatternPronounTo[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsPatternPronounsToFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsPatternPronounTo.Count; i++) {
                    ItemPatternPronoun item = itemsPatternPronounTo[i];
                    itemsPatternPronounsToFiltered.Add(item);
                }
            }
        }
               
        void PatternPronounToAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternPronounToSaveCurrent();

            var newItem=new ItemPatternPronoun();
           // newItem.ID=itemsPronouns.Count;
            itemsPatternPronounTo.Add(newItem);
            CurrentPatternPronounTo=newItem;
            PatternPronounToRefreshFilteredList();
            PatternPronounToSetListBox(); 
            PatternPronounToListBoxSetCurrent();
            PatternPronounToSetCurrent(); 
            
            doingJob=false;
        }
   
        void PatternPronounToRemoveItem(ItemPatternPronoun item) { 
            Edited=true;
            ChangeCaptionText();
            itemsPatternPronounTo.Remove(item);
            PatternPronounToRefreshFilteredList();
            PatternPronounToSetListBox();
            PatternPronounToSetCurrent();
        } 
           
        void PatternPronounToSetCurrent(){
            if (itemsPatternPronounsToFiltered.Count==0) {
                PatternPronounToSetNone();
                return;
            }

            int index=listBoxPatternPronounTo.SelectedIndex;
            if (index>=itemsPatternPronounsToFiltered.Count) index=itemsPatternPronounsToFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternPronounTo=itemsPatternPronounsToFiltered[index];
            
            textBoxPatternPronounName.Text=CurrentPatternPronounTo.Name;
            comboBoxPatternPronounType.SelectedIndex=(int)CurrentPatternPronounTo.Type;
          //  comboBoxPatternPronounGender.SelectedIndex=(int)CurrentPatternPronoun.Gender;

            //textBoxPatternPronounMuzJ1.Text=CurrentPatternPronoun.Shapes[0];
            //textBoxPatternPronounMuzJ2.Text=CurrentPatternPronoun.Shapes[1];
            //textBoxPatternPronounMuzJ3.Text=CurrentPatternPronoun.Shapes[2];
            //textBoxPatternPronounMuzJ4.Text=CurrentPatternPronoun.Shapes[3];
            //textBoxPatternPronounMuzJ5.Text=CurrentPatternPronoun.Shapes[4];
            //textBoxPatternPronounMuzJ6.Text=CurrentPatternPronoun.Shapes[5];
            //textBoxPatternPronounMuzJ7.Text=CurrentPatternPronoun.Shapes[6];

            //textBoxPatternPronounMuzM1.Text=CurrentPatternPronoun.Shapes[7];
            //textBoxPatternPronounMuzM2.Text=CurrentPatternPronoun.Shapes[8];
            //textBoxPatternPronounMuzM3.Text=CurrentPatternPronoun.Shapes[9];
            //textBoxPatternPronounMuzM4.Text=CurrentPatternPronoun.Shapes[10];
            //textBoxPatternPronounMuzM5.Text=CurrentPatternPronoun.Shapes[11];
            //textBoxPatternPronounMuzM6.Text=CurrentPatternPronoun.Shapes[12];
            //textBoxPatternPronounMuzM7.Text=CurrentPatternPronoun.Shapes[13];

             switch (CurrentPatternPronounTo.Type) { 
                case PronounType.Unknown:
                    break;
                    
                case PronounType.NoDeklination:
                     textBoxPatternPronounFromMuzJ1.Text=CurrentPatternPronounTo.Shapes[0];
                    break;

                case PronounType.DeklinationOnlySingle:
                    textBoxPatternPronounFromMuzJ1.Text=CurrentPatternPronounTo.Shapes[0];
                    textBoxPatternPronounFromMuzJ2.Text=CurrentPatternPronounTo.Shapes[1];
                    textBoxPatternPronounFromMuzJ3.Text=CurrentPatternPronounTo.Shapes[2];
                    textBoxPatternPronounFromMuzJ4.Text=CurrentPatternPronounTo.Shapes[3];
                    textBoxPatternPronounFromMuzJ5.Text=CurrentPatternPronounTo.Shapes[4];
                    textBoxPatternPronounFromMuzJ6.Text=CurrentPatternPronounTo.Shapes[5];
                    textBoxPatternPronounFromMuzJ7.Text=CurrentPatternPronounTo.Shapes[6];
                    break;
                    
                case PronounType.Deklination:
                    textBoxPatternPronounFromMuzJ1.Text=CurrentPatternPronounTo.Shapes[0];
                    textBoxPatternPronounFromMuzJ2.Text=CurrentPatternPronounTo.Shapes[1];
                    textBoxPatternPronounFromMuzJ3.Text=CurrentPatternPronounTo.Shapes[2];
                    textBoxPatternPronounFromMuzJ4.Text=CurrentPatternPronounTo.Shapes[3];
                    textBoxPatternPronounFromMuzJ5.Text=CurrentPatternPronounTo.Shapes[4];
                    textBoxPatternPronounFromMuzJ6.Text=CurrentPatternPronounTo.Shapes[5];
                    textBoxPatternPronounFromMuzJ7.Text=CurrentPatternPronounTo.Shapes[6];

                    textBoxPatternPronounFromMuzM1.Text=CurrentPatternPronounTo.Shapes[7];
                    textBoxPatternPronounFromMuzM2.Text=CurrentPatternPronounTo.Shapes[8];
                    textBoxPatternPronounFromMuzM3.Text=CurrentPatternPronounTo.Shapes[9];
                    textBoxPatternPronounFromMuzM4.Text=CurrentPatternPronounTo.Shapes[10];
                    textBoxPatternPronounFromMuzM5.Text=CurrentPatternPronounTo.Shapes[11];
                    textBoxPatternPronounFromMuzM6.Text=CurrentPatternPronounTo.Shapes[12];
                    textBoxPatternPronounFromMuzM7.Text=CurrentPatternPronounTo.Shapes[13];
                    break;
                    
                case PronounType.DeklinationWithGender:
                    textBoxPatternPronounFromMuzJ1.Text=CurrentPatternPronounTo.Shapes[0] ;
                    textBoxPatternPronounFromMuzJ2.Text=CurrentPatternPronounTo.Shapes[1] ;
                    textBoxPatternPronounFromMuzJ3.Text=CurrentPatternPronounTo.Shapes[2] ;
                    textBoxPatternPronounFromMuzJ4.Text=CurrentPatternPronounTo.Shapes[3] ;
                    textBoxPatternPronounFromMuzJ5.Text=CurrentPatternPronounTo.Shapes[4] ;
                    textBoxPatternPronounFromMuzJ6.Text=CurrentPatternPronounTo.Shapes[5] ;
                    textBoxPatternPronounFromMuzJ7.Text=CurrentPatternPronounTo.Shapes[6] ;
                    textBoxPatternPronounFromMuzM1.Text=CurrentPatternPronounTo.Shapes[7] ;
                    textBoxPatternPronounFromMuzM2.Text=CurrentPatternPronounTo.Shapes[8] ;
                    textBoxPatternPronounFromMuzM3.Text=CurrentPatternPronounTo.Shapes[9] ;
                    textBoxPatternPronounFromMuzM4.Text=CurrentPatternPronounTo.Shapes[10];
                    textBoxPatternPronounFromMuzM5.Text=CurrentPatternPronounTo.Shapes[11];
                    textBoxPatternPronounFromMuzM6.Text=CurrentPatternPronounTo.Shapes[12];
                    textBoxPatternPronounFromMuzM7.Text=CurrentPatternPronounTo.Shapes[13];

                    textBoxPatternPronounFromMunJ1.Text=CurrentPatternPronounTo.Shapes[14];
                    textBoxPatternPronounFromMunJ2.Text=CurrentPatternPronounTo.Shapes[15];
                    textBoxPatternPronounMunJ3.Text=CurrentPatternPronounTo.Shapes[16];
                    textBoxPatternPronounFromMunJ4.Text=CurrentPatternPronounTo.Shapes[17];
                    textBoxPatternPronounFromMunJ5.Text=CurrentPatternPronounTo.Shapes[18];
                    textBoxPatternPronounFromMunJ6.Text=CurrentPatternPronounTo.Shapes[19];
                    textBoxPatternPronounFromMunJ7.Text=CurrentPatternPronounTo.Shapes[20];
                    textBoxPatternPronounMunM1.Text=CurrentPatternPronounTo.Shapes[21];
                    textBoxPatternPronounMunM2.Text=CurrentPatternPronounTo.Shapes[22];
                    textBoxPatternPronounMunM3.Text=CurrentPatternPronounTo.Shapes[23];
                    textBoxPatternPronounMunM4.Text=CurrentPatternPronounTo.Shapes[24];
                    textBoxPatternPronounMunM5.Text=CurrentPatternPronounTo.Shapes[25];
                    textBoxPatternPronounMunM6.Text=CurrentPatternPronounTo.Shapes[26];
                    textBoxPatternPronounFromMunM7.Text=CurrentPatternPronounTo.Shapes[27];

                    textBoxPatternPronounZenJ1.Text=CurrentPatternPronounTo.Shapes[28];
                    textBoxPatternPronounZenJ2.Text=CurrentPatternPronounTo.Shapes[29];
                    textBoxPatternPronounZenJ3.Text=CurrentPatternPronounTo.Shapes[30];
                    textBoxPatternPronounZenJ4.Text=CurrentPatternPronounTo.Shapes[31];
                    textBoxPatternPronounZenJ5.Text=CurrentPatternPronounTo.Shapes[32];
                    textBoxPatternPronounZenJ6.Text=CurrentPatternPronounTo.Shapes[33];
                    textBoxPatternPronounZenJ7.Text=CurrentPatternPronounTo.Shapes[34];
                    textBoxPatternPronounZenM1.Text=CurrentPatternPronounTo.Shapes[35];
                    textBoxPatternPronounZenM2.Text=CurrentPatternPronounTo.Shapes[36];
                    textBoxPatternPronounZenM3.Text=CurrentPatternPronounTo.Shapes[37];
                    textBoxPatternPronounZenM4.Text=CurrentPatternPronounTo.Shapes[38];
                    textBoxPatternPronounZenM5.Text=CurrentPatternPronounTo.Shapes[39];
                    textBoxPatternPronounZenM6.Text=CurrentPatternPronounTo.Shapes[40];
                    textBoxPatternPronounZenM7.Text=CurrentPatternPronounTo.Shapes[41];

                    textBoxPatternPronounStrJ1.Text=CurrentPatternPronounTo.Shapes[42];
                    textBoxPatternPronounStrJ2.Text=CurrentPatternPronounTo.Shapes[43];
                    textBoxPatternPronounStrJ3.Text=CurrentPatternPronounTo.Shapes[44];
                    textBoxPatternPronounStrJ4.Text=CurrentPatternPronounTo.Shapes[45];
                    textBoxPatternPronounStrJ5.Text=CurrentPatternPronounTo.Shapes[46];
                    textBoxPatternPronounStrJ6.Text=CurrentPatternPronounTo.Shapes[47];
                    textBoxPatternPronounStrJ7.Text=CurrentPatternPronounTo.Shapes[48];
                    textBoxPatternPronounStrM1.Text=CurrentPatternPronounTo.Shapes[49];
                    textBoxPatternPronounStrM2.Text=CurrentPatternPronounTo.Shapes[50];
                    textBoxPatternPronounStrM3.Text=CurrentPatternPronounTo.Shapes[51];
                    textBoxPatternPronounStrM4.Text=CurrentPatternPronounTo.Shapes[52];
                    textBoxPatternPronounStrM5.Text=CurrentPatternPronounTo.Shapes[53];
                    textBoxPatternPronounStrM6.Text=CurrentPatternPronounTo.Shapes[54];
                    textBoxPatternPronounStrM7.Text=CurrentPatternPronounTo.Shapes[55];
                    break;
            }

            labelPatternPronounName.Visible=true;
            textBoxPatternPronounName.Visible=true;
            labelPatternPronounType.Visible=true;
            comboBoxPatternPronounType.Visible=true;

            labelPatternPronounMuzFall.Visible=true;
            labelPatternPronounMuzSingle.Visible=true;
            labelPatternPronounMuzMultiple.Visible=true;
            tableLayoutPanelPatternPronounMuz.Visible=true; 
            
           //abelPatternPronounInfo.Visible=true;
            ChangeTypePatternPronoun(CurrentPatternPronounTo?.Type);
        }
         
        void PatternPronounToListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternPronounsToFiltered.Count; indexCur++) {
                if (itemsPatternPronounTo[indexCur]==CurrentPatternPronounTo) { 
                    int indexList=listBoxPatternPronounTo.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxPatternPronounTo.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void PatternPronounToSaveCurrent() {
            if (CurrentPatternPronounTo==null) return;
            comboBoxPatternPronounType.SelectedIndex=(int) CurrentPatternPronounTo.Type;
            CurrentPatternPronounTo.Name=textBoxPatternPronounName.Text;
            switch (CurrentPatternPronounTo.Type) { 
                case PronounType.Unknown:
                    break;

                case PronounType.NoDeklination:
                    CurrentPatternPronounTo.Shapes[0]=textBoxPatternPronounFromMuzJ1.Text;
                    break;
                    
                case PronounType.DeklinationOnlySingle:
                    CurrentPatternPronounTo.Shapes[0]=textBoxPatternPronounFromMuzJ1.Text;
                    CurrentPatternPronounTo.Shapes[1]=textBoxPatternPronounFromMuzJ2.Text;
                    CurrentPatternPronounTo.Shapes[2]=textBoxPatternPronounFromMuzJ3.Text;
                    CurrentPatternPronounTo.Shapes[3]=textBoxPatternPronounFromMuzJ4.Text;
                    CurrentPatternPronounTo.Shapes[4]=textBoxPatternPronounFromMuzJ5.Text;
                    CurrentPatternPronounTo.Shapes[5]=textBoxPatternPronounFromMuzJ6.Text;
                    CurrentPatternPronounTo.Shapes[6]=textBoxPatternPronounFromMuzJ7.Text;
                    break;
                    
                case PronounType.Deklination:
                    CurrentPatternPronounTo.Shapes[0]=textBoxPatternPronounFromMuzJ1.Text;
                    CurrentPatternPronounTo.Shapes[1]=textBoxPatternPronounFromMuzJ2.Text;
                    CurrentPatternPronounTo.Shapes[2]=textBoxPatternPronounFromMuzJ3.Text;
                    CurrentPatternPronounTo.Shapes[3]=textBoxPatternPronounFromMuzJ4.Text;
                    CurrentPatternPronounTo.Shapes[4]=textBoxPatternPronounFromMuzJ5.Text;
                    CurrentPatternPronounTo.Shapes[5]=textBoxPatternPronounFromMuzJ6.Text;
                    CurrentPatternPronounTo.Shapes[6]=textBoxPatternPronounFromMuzJ7.Text;

                    CurrentPatternPronounTo.Shapes[7]=textBoxPatternPronounFromMuzM1.Text;
                    CurrentPatternPronounTo.Shapes[8]=textBoxPatternPronounFromMuzM2.Text;
                    CurrentPatternPronounTo.Shapes[9]=textBoxPatternPronounFromMuzM3.Text;
                    CurrentPatternPronounTo.Shapes[10]=textBoxPatternPronounFromMuzM4.Text;
                    CurrentPatternPronounTo.Shapes[11]=textBoxPatternPronounFromMuzM5.Text;
                    CurrentPatternPronounTo.Shapes[12]=textBoxPatternPronounFromMuzM6.Text;
                    CurrentPatternPronounTo.Shapes[13]=textBoxPatternPronounFromMuzM7.Text;
                    break;
                    
                case PronounType.DeklinationWithGender:
                    CurrentPatternPronounTo.Shapes[0]=textBoxPatternPronounFromMuzJ1.Text;
                    CurrentPatternPronounTo.Shapes[1]=textBoxPatternPronounFromMuzJ2.Text;
                    CurrentPatternPronounTo.Shapes[2]=textBoxPatternPronounFromMuzJ3.Text;
                    CurrentPatternPronounTo.Shapes[3]=textBoxPatternPronounFromMuzJ4.Text;
                    CurrentPatternPronounTo.Shapes[4]=textBoxPatternPronounFromMuzJ5.Text;
                    CurrentPatternPronounTo.Shapes[5]=textBoxPatternPronounFromMuzJ6.Text;
                    CurrentPatternPronounTo.Shapes[6]=textBoxPatternPronounFromMuzJ7.Text;
                    CurrentPatternPronounTo.Shapes[7]=textBoxPatternPronounFromMuzM1.Text;
                    CurrentPatternPronounTo.Shapes[8]=textBoxPatternPronounFromMuzM2.Text;
                    CurrentPatternPronounTo.Shapes[9]=textBoxPatternPronounFromMuzM3.Text;
                    CurrentPatternPronounTo.Shapes[10]=textBoxPatternPronounFromMuzM4.Text;
                    CurrentPatternPronounTo.Shapes[11]=textBoxPatternPronounFromMuzM5.Text;
                    CurrentPatternPronounTo.Shapes[12]=textBoxPatternPronounFromMuzM6.Text;
                    CurrentPatternPronounTo.Shapes[13]=textBoxPatternPronounFromMuzM7.Text;

                    CurrentPatternPronounTo.Shapes[14]=textBoxPatternPronounFromMunJ1.Text;
                    CurrentPatternPronounTo.Shapes[15]=textBoxPatternPronounFromMunJ2.Text;
                    CurrentPatternPronounTo.Shapes[16]=textBoxPatternPronounMunJ3.Text;
                    CurrentPatternPronounTo.Shapes[17]=textBoxPatternPronounFromMunJ4.Text;
                    CurrentPatternPronounTo.Shapes[18]=textBoxPatternPronounFromMunJ5.Text;
                    CurrentPatternPronounTo.Shapes[19]=textBoxPatternPronounFromMunJ6.Text;
                    CurrentPatternPronounTo.Shapes[20]=textBoxPatternPronounFromMunJ7.Text;
                    CurrentPatternPronounTo.Shapes[21]=textBoxPatternPronounMunM1.Text;
                    CurrentPatternPronounTo.Shapes[22]=textBoxPatternPronounMunM2.Text;
                    CurrentPatternPronounTo.Shapes[23]=textBoxPatternPronounMunM3.Text;
                    CurrentPatternPronounTo.Shapes[24]=textBoxPatternPronounMunM4.Text;
                    CurrentPatternPronounTo.Shapes[25]=textBoxPatternPronounMunM5.Text;
                    CurrentPatternPronounTo.Shapes[26]=textBoxPatternPronounMunM6.Text;
                    CurrentPatternPronounTo.Shapes[27]=textBoxPatternPronounFromMunM7.Text;

                    CurrentPatternPronounTo.Shapes[28]=textBoxPatternPronounZenJ1.Text;
                    CurrentPatternPronounTo.Shapes[29]=textBoxPatternPronounZenJ2.Text;
                    CurrentPatternPronounTo.Shapes[30]=textBoxPatternPronounZenJ3.Text;
                    CurrentPatternPronounTo.Shapes[31]=textBoxPatternPronounZenJ4.Text;
                    CurrentPatternPronounTo.Shapes[32]=textBoxPatternPronounZenJ5.Text;
                    CurrentPatternPronounTo.Shapes[33]=textBoxPatternPronounZenJ6.Text;
                    CurrentPatternPronounTo.Shapes[34]=textBoxPatternPronounZenJ7.Text;
                    CurrentPatternPronounTo.Shapes[35]=textBoxPatternPronounZenM1.Text;
                    CurrentPatternPronounTo.Shapes[36]=textBoxPatternPronounZenM2.Text;
                    CurrentPatternPronounTo.Shapes[37]=textBoxPatternPronounZenM3.Text;
                    CurrentPatternPronounTo.Shapes[38]=textBoxPatternPronounZenM4.Text;
                    CurrentPatternPronounTo.Shapes[39]=textBoxPatternPronounZenM5.Text;
                    CurrentPatternPronounTo.Shapes[40]=textBoxPatternPronounZenM6.Text;
                    CurrentPatternPronounTo.Shapes[41]=textBoxPatternPronounZenM7.Text;

                    CurrentPatternPronounTo.Shapes[42]=textBoxPatternPronounStrJ1.Text;
                    CurrentPatternPronounTo.Shapes[43]=textBoxPatternPronounStrJ2.Text;
                    CurrentPatternPronounTo.Shapes[44]=textBoxPatternPronounStrJ3.Text;
                    CurrentPatternPronounTo.Shapes[45]=textBoxPatternPronounStrJ4.Text;
                    CurrentPatternPronounTo.Shapes[46]=textBoxPatternPronounStrJ5.Text;
                    CurrentPatternPronounTo.Shapes[47]=textBoxPatternPronounStrJ6.Text;
                    CurrentPatternPronounTo.Shapes[48]=textBoxPatternPronounStrJ7.Text;
                    CurrentPatternPronounTo.Shapes[49]=textBoxPatternPronounStrM1.Text;
                    CurrentPatternPronounTo.Shapes[50]=textBoxPatternPronounStrM2.Text;
                    CurrentPatternPronounTo.Shapes[51]=textBoxPatternPronounStrM3.Text;
                    CurrentPatternPronounTo.Shapes[52]=textBoxPatternPronounStrM4.Text;
                    CurrentPatternPronounTo.Shapes[53]=textBoxPatternPronounStrM5.Text;
                    CurrentPatternPronounTo.Shapes[54]=textBoxPatternPronounStrM6.Text;
                    CurrentPatternPronounTo.Shapes[55]=textBoxPatternPronounStrM7.Text;
                    break;
            }
        } 
              
        void PatternPronounToSetNone(){ 
            textBoxPatternPronounName.Text="";

            textBoxPatternPronounFromMuzJ1.Text="";
            textBoxPatternPronounFromMuzJ2.Text="";
            textBoxPatternPronounFromMuzJ3.Text="";
            textBoxPatternPronounFromMuzJ4.Text="";
            textBoxPatternPronounFromMuzJ5.Text="";
            textBoxPatternPronounFromMuzJ6.Text="";
            textBoxPatternPronounFromMuzJ7.Text="";

            textBoxPatternPronounFromMuzM1.Text="";
            textBoxPatternPronounFromMuzM2.Text="";
            textBoxPatternPronounFromMuzM3.Text="";
            textBoxPatternPronounFromMuzM4.Text="";
            textBoxPatternPronounFromMuzM5.Text="";
            textBoxPatternPronounFromMuzM6.Text="";
            textBoxPatternPronounFromMuzM7.Text="";

            
            textBoxPatternPronounZenJ1.Text="";
            textBoxPatternPronounZenJ2.Text="";
            textBoxPatternPronounZenJ3.Text="";
            textBoxPatternPronounZenJ4.Text="";
            textBoxPatternPronounZenJ5.Text="";
            textBoxPatternPronounZenJ6.Text="";
            textBoxPatternPronounZenJ7.Text="";

            textBoxPatternPronounZenM1.Text="";
            textBoxPatternPronounZenM2.Text="";
            textBoxPatternPronounZenM3.Text="";
            textBoxPatternPronounZenM4.Text="";
            textBoxPatternPronounZenM5.Text="";
            textBoxPatternPronounZenM6.Text="";
            textBoxPatternPronounZenM7.Text="";

            textBoxPatternPronounFromMunJ1.Text="";
            textBoxPatternPronounFromMunJ2.Text="";
            textBoxPatternPronounMunJ3.Text="";
            textBoxPatternPronounFromMunJ4.Text="";
            textBoxPatternPronounFromMunJ5.Text="";
            textBoxPatternPronounFromMunJ6.Text="";
            textBoxPatternPronounFromMunJ7.Text="";

            textBoxPatternPronounMunM1.Text="";
            textBoxPatternPronounMunM2.Text="";
            textBoxPatternPronounMunM3.Text="";
            textBoxPatternPronounMunM4.Text="";
            textBoxPatternPronounMunM5.Text="";
            textBoxPatternPronounMunM6.Text="";
            textBoxPatternPronounFromMunM7.Text="";

            textBoxPatternPronounStrJ1.Text="";
            textBoxPatternPronounStrJ2.Text="";
            textBoxPatternPronounStrJ3.Text="";
            textBoxPatternPronounStrJ4.Text="";
            textBoxPatternPronounStrJ5.Text="";
            textBoxPatternPronounStrJ6.Text="";
            textBoxPatternPronounStrJ7.Text="";

            textBoxPatternPronounStrM1.Text="";
            textBoxPatternPronounStrM2.Text="";
            textBoxPatternPronounStrM3.Text="";
            textBoxPatternPronounStrM4.Text="";
            textBoxPatternPronounStrM5.Text="";
            textBoxPatternPronounStrM6.Text="";
            textBoxPatternPronounStrM7.Text="";

            labelPatternPronounName.Visible=false;
            textBoxPatternPronounName.Visible=false;
            labelPatternPronounType.Visible=false;
            comboBoxPatternPronounType.Visible=false;

            labelPatternPronounMuzFall.Visible=false;
            labelPatternPronounMuzSingle.Visible=false;
            labelPatternPronounMuzMultiple.Visible=false;
            tableLayoutPanelPatternPronounMuz.Visible=false;
            //labelPatternPronounInfo.Visible=false;
           // ChangeTypePatternPronoun(CurrentPatternPronoun?.Type);
        }
                
        void PatternPronounToClear() { 
            listBoxPatternPronounTo.Items.Clear();
            PatternPronounToSetNone();
            itemsPatternPronounsToFiltered?.Clear();
            itemsPatternPronounTo?.Clear();
            CurrentPatternPronounTo=null;
        }
        #endregion

        #region Pronoun
        void ListBoxPronoun_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentPronoun();
            
            int index=listBoxPronoun.SelectedIndex;
            if (itemsPronouns.Count==0) {
                PronounSetNone();
                return;
            }
            if (index>=itemsPronouns.Count) 
                index=itemsPronouns.Count-1;
            if (index<0)
                index=0;
           
            CurrentPronoun=itemsPronouns[index];
            SetCurrentPronoun();
            SetListBoxPronoun();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void ButtonPronounAdd_Click(object sender, EventArgs e) {
            AddNewItemPronoun();
        }

        void ButtonPronounRemove_Click(object sender, EventArgs e) {
            RemoveItemPronoun(CurrentPronoun);
            TextBoxPronounFilter_TextChanged(null, new EventArgs());
        }
        
        void TextBoxPronounFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentPronoun();

            // Získej aktuální prvek
            ItemPronoun selectedId=null;
            if (listBoxPronoun.SelectedIndex!=-1) {
                selectedId=itemsPronounsFiltered[listBoxPronoun.SelectedIndex];
            }
            
            PronounRefreshFilteredList();

            listBoxPronoun.Items.Clear();
            for (int i=0; i<itemsPronounsFiltered.Count; i++) {
                ItemPronoun item = itemsPronounsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPronoun.Items.Add(textToAdd);
            }

            //SetListBoxPronoun();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsPronounsFiltered.Count; i++){
                    if (itemsPronounsFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxPronoun.SelectedIndex=-1;
                    CurrentPronoun=null;
                } else listBoxPronoun.SelectedIndex=outIndex;
            } else {
                listBoxPronoun.SelectedIndex=-1;
                CurrentPronoun=null;
            }
            SetCurrentPronoun();
        }
            
        void RemoveCurrentPronoun(object sender, EventArgs e) {
            itemsPronouns.Remove(CurrentPronoun);
        }

        void SetListBoxPronoun() { 
            string filter=textBoxPronounFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxPronoun.SelectedIndex;
            listBoxPronoun.Items.Clear();
            for (int i=0; i<itemsPronounsFiltered.Count; i++) {
                ItemPronoun item = itemsPronounsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPronoun.Items.Add(textToAdd);
            }

            if (index>=listBoxPronoun.Items.Count)index=listBoxPronoun.Items.Count-1;
            listBoxPronoun.SelectedIndex=index;
        }

        void PronounRefreshFilteredList() {
            if (itemsPronounsFiltered==null) itemsPronounsFiltered=new List<ItemPronoun>();
            itemsPronounsFiltered.Clear();
            string filter=textBoxPronounFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsPronouns.Count; i++) {
                    ItemPronoun item = itemsPronouns[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsPronounsFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsPronouns.Count; i++) {
                    ItemPronoun item = itemsPronouns[i];
                    itemsPronounsFiltered.Add(item);
                }
            }
        }
               
        void AddNewItemPronoun() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentPronoun();

            var newItem=new ItemPronoun();
           // newItem.ID=itemsPronouns.Count;
            itemsPronouns.Add(newItem);
            CurrentPronoun=newItem;
            PronounRefreshFilteredList();
            SetListBoxPronoun(); 
            ListBoxSetCurrentPronoun();
            SetCurrentPronoun(); 
            
            doingJob=false;
        }
   
        void RemoveItemPronoun(ItemPronoun item) { 
            Edited=true;
            ChangeCaptionText();
            itemsPronouns.Remove(item);
            PronounRefreshFilteredList();
            SetListBoxPronoun();
            SetCurrentPronoun();
        } 
           
        void SetCurrentPronoun(){
            if (itemsPronounsFiltered.Count==0) {
                PronounSetNone();
                return;
            }

            int index=listBoxPronoun.SelectedIndex;
            if (index>=itemsPronounsFiltered.Count) index=itemsPronounsFiltered.Count-1;
            if (index<0) index=0;
            CurrentPronoun=itemsPronounsFiltered[index];
            
            textBoxPronounFrom.Visible=true;
            textBoxPronounTo.Visible=true;
            labelPronounFrom.Visible=true;
            labelPronounTo.Visible=true; 

            textBoxPronounFrom.Text=CurrentPronoun.From;
            textBoxPronounTo.Text=CurrentPronoun.To;

            comboBoxPronounInputPatternFrom.Text=CurrentPronoun.PatternFrom;

            comboBoxPronounInputPatternFrom.Items.Clear();
            comboBoxPronounInputPatternTo.Items.Clear();
            foreach (ItemPatternPronoun x in itemsPatternPronounFrom) {
                comboBoxPronounInputPatternFrom.Items.Add(x.Name);
                comboBoxPronounInputPatternTo.Items.Add(x.Name);
            }
            
            comboBoxPronounInputPatternTo.Text=CurrentPronoun.PatternTo;
      
            comboBoxPronounInputPatternFrom.Visible=true; 
            comboBoxPronounInputPatternTo.Visible=true; 

            labelPronounInputPatternFrom.Visible=true; 
            labelPronounInputPatternTo.Visible=true; 

            labelPronounShowFrom.Visible=true;
            labelPronounShowTo.Visible=true;
        }
         
        void ListBoxSetCurrentPronoun() {
            for (int indexCur=0; indexCur<itemsPronounsFiltered.Count; indexCur++) {
                if (itemsPronouns[indexCur]==CurrentPronoun) { 
                    int indexList=listBoxPronoun.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxPronoun.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void SaveCurrentPronoun() {
            if (CurrentPronoun==null) return;
                     
            CurrentPronoun.From=textBoxPronounFrom.Text;
            CurrentPronoun.To=textBoxPronounTo.Text;
            
            CurrentPronoun.PatternFrom=comboBoxPronounInputPatternFrom.Text;
            CurrentPronoun.PatternTo=comboBoxPronounInputPatternTo.Text;
        } 
              
        void PronounSetNone(){ 
            textBoxPronounFrom.Text="";
            textBoxPronounTo.Text="";

            comboBoxPronounInputPatternFrom.Text="";
            comboBoxPronounInputPatternTo.Text="";

            textBoxPronounFrom.Visible=false;
            textBoxPronounTo.Visible=false;
            labelPronounFrom.Visible=false;
            labelPronounTo.Visible=false;
            comboBoxPronounInputPatternTo.Visible=false;
            comboBoxPronounInputPatternFrom.Visible=false;
            labelPronounShowFrom.Visible=false;
            labelPronounShowTo.Visible=false;
            labelPronounInputPatternFrom.Visible=false; 
            labelPronounInputPatternTo.Visible=false; 
        }
        
        void ClearPronoun(){ 
            listBoxPronoun.Items.Clear();
            PronounSetNone();
            itemsPronounsFiltered?.Clear();
            itemsPronouns?.Clear();
            CurrentPronoun=null;
        }
        #endregion
 
        #region NumberPattern From
        void PatternNumberFromListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternNumberFromSaveCurrent();
            
            int index=PatternNumberFromlistBox.SelectedIndex;
            if (itemsPatternNumbersFrom.Count==0) {
                PatternNumberFromSetNone();
                return;
            }
            if (index>=itemsPatternNumbersFrom.Count) 
                index=itemsPatternNumbersFrom.Count-1;
            if (index<0)
                index=0;
           
            CurrentPatternFromNumber=itemsPatternNumbersFrom[index];
            PatternNumberFromSetCurrent();
            PatternNumberFromSetListBox();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void PatternNumberFromButtonAdd_Click(object sender, EventArgs e) {
            PatternNumberFromAddNewItem();
        }

        void PatternNumberFromButtonRemove_Click(object sender, EventArgs e) {
            PatternNumberFromRemoveItem(CurrentPatternFromNumber);
            PatternNumberFromTextBoxFilter_TextChanged(null, new EventArgs());
        }
        
        void PatternNumberFromTextBoxFilter_TextChanged(object sender, EventArgs e) {
            PatternNumberFromSaveCurrent();

            // Získej aktuální prvek
            ItemPatternNumber selectedId=null;
            if (PatternNumberFromlistBox.SelectedIndex!=-1) {
                selectedId=itemsPatternNumbersFromFiltered[PatternNumberFromlistBox.SelectedIndex];
            }
            
            PatternNumberFromRefreshFilteredList();

            PatternNumberFromlistBox.Items.Clear();
            for (int i=0; i<itemsPatternNumbersFromFiltered.Count; i++) {
                ItemPatternNumber item = itemsPatternNumbersFromFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                PatternNumberFromlistBox.Items.Add(textToAdd);
            }

            //SetListBoxNumber();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsPatternNumbersFromFiltered.Count; i++){
                    if (itemsPatternNumbersFromFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    PatternNumberFromlistBox.SelectedIndex=-1;
                    CurrentPatternFromNumber=null;
                } else PatternNumberFromlistBox.SelectedIndex=outIndex;
            } else {
                PatternNumberFromlistBox.SelectedIndex=-1;
                CurrentPatternFromNumber=null;
            }
            PatternNumberFromSetCurrent();
        }
            
        void PatternNumberFromRemoveCurrent(object sender, EventArgs e) {
            itemsPatternNumbersFrom.Remove(CurrentPatternFromNumber);
        }

        void PatternNumberFromSetListBox() { 
            string filter=textBoxPatternNumberFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=PatternNumberFromlistBox.SelectedIndex;
            PatternNumberFromlistBox.Items.Clear();
            for (int i=0; i<itemsPatternNumbersFromFiltered.Count; i++) {
                ItemPatternNumber item = itemsPatternNumbersFromFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                PatternNumberFromlistBox.Items.Add(textToAdd);
            }

            if (index>=PatternNumberFromlistBox.Items.Count)index=PatternNumberFromlistBox.Items.Count-1;
            PatternNumberFromlistBox.SelectedIndex=index;
        }

        void PatternNumberFromRefreshFilteredList() {
            if (itemsPatternNumbersFromFiltered==null) itemsPatternNumbersFromFiltered=new List<ItemPatternNumber>();
            itemsPatternNumbersFromFiltered.Clear();
            string filter=textBoxPatternNumberFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsPatternNumbersFrom.Count; i++) {
                    ItemPatternNumber item = itemsPatternNumbersFrom[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsPatternNumbersFromFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsPatternNumbersFrom.Count; i++) {
                    ItemPatternNumber item = itemsPatternNumbersFrom[i];
                    itemsPatternNumbersFromFiltered.Add(item);
                }
            }
        }
               
        void PatternNumberFromAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternNumberFromSaveCurrent();

            var newItem=new ItemPatternNumber();
           // newItem.ID=itemsNumbers.Count;
            itemsPatternNumbersFrom.Add(newItem);
            CurrentPatternFromNumber=newItem;
            PatternNumberFromRefreshFilteredList();
            PatternNumberFromSetListBox(); 
            PatternNumberFromListBoxSetCurrent();
            PatternNumberFromSetCurrent(); 
            
            doingJob=false;
        }
   
        void PatternNumberFromRemoveItem(ItemPatternNumber item) { 
            Edited=true;
            ChangeCaptionText();
            itemsPatternNumbersFrom.Remove(item);
            PatternNumberFromRefreshFilteredList();
            PatternNumberFromSetListBox();
            PatternNumberFromSetCurrent();
        } 
           
        void PatternNumberFromSetCurrent(){
            if (itemsPatternNumbersFromFiltered.Count==0) {
                PatternNumberFromSetNone();
                return;
            }

            int index=PatternNumberFromlistBox.SelectedIndex;
            if (index>=itemsPatternNumbersFromFiltered.Count) index=itemsPatternNumbersFromFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternFromNumber=itemsPatternNumbersFromFiltered[index];
            textBoxPatternNumberName.Text=CurrentPatternFromNumber.Name;
            comboBoxPatternNumberType.SelectedIndex=(int)CurrentPatternFromNumber.ShowType;
            
            if (CurrentPatternFromNumber.ShowType!=NumberType.Unknown) {
                if (CurrentPatternFromNumber.ShowType==NumberType.NoDeklination || CurrentPatternFromNumber.ShowType==NumberType.Deklination || CurrentPatternFromNumber.ShowType==NumberType.DeklinationOnlySingle || CurrentPatternFromNumber.ShowType==NumberType.DeklinationWithGender) {
                    textBoxPatternNumberFromMuzS1.Text=CurrentPatternFromNumber.Shapes[0];
                    textBoxPatternNumberFromMuzS1.Visible=true;       
                    
                    textBoxPatternNumberName.Visible=true;
                    labelPatternNumberMuzFall.Visible=true;
                    labelPatternNumberMuzMultiple.Visible=true;
                    labelPatternNumberMuzSingle.Visible=true;
                } else { 
                    textBoxPatternNumberFromMuzS1.Visible=false;
                    textBoxPatternNumberName.Visible=false;
                    labelPatternNumberMuzFall.Visible=false;
                    labelPatternNumberMuzMultiple.Visible=false;
                    labelPatternNumberMuzSingle.Visible=false;
                }
                if (CurrentPatternFromNumber.ShowType==NumberType.Deklination || CurrentPatternFromNumber.ShowType==NumberType.DeklinationOnlySingle || CurrentPatternFromNumber.ShowType==NumberType.DeklinationWithGender) {
                    textBoxPatternNumberFromMuzS2.Text=CurrentPatternFromNumber.Shapes[1];
                    textBoxPatternNumberFromMuzS3.Text=CurrentPatternFromNumber.Shapes[2];
                    textBoxPatternNumberMuzS4.Text=CurrentPatternFromNumber.Shapes[3];
                    textBoxPatternNumberMuzS5.Text=CurrentPatternFromNumber.Shapes[4];
                    textBoxPatternNumberMuzS6.Text=CurrentPatternFromNumber.Shapes[5];
                    textBoxPatternNumberMuzS7.Text=CurrentPatternFromNumber.Shapes[6];
                    textBoxPatternNumberFromMuzS2.Visible=true;
                    textBoxPatternNumberFromMuzS3.Visible=true;
                    textBoxPatternNumberMuzS4.Visible=true;
                    textBoxPatternNumberMuzS5.Visible=true;
                    textBoxPatternNumberMuzS6.Visible=true;
                    textBoxPatternNumberMuzS7.Visible=true;
                } else { 
                    textBoxPatternNumberFromMuzS2.Visible=false;
                    textBoxPatternNumberFromMuzS3.Visible=false;
                    textBoxPatternNumberMuzS4.Visible=false;
                    textBoxPatternNumberMuzS5.Visible=false;
                    textBoxPatternNumberMuzS6.Visible=false;
                    textBoxPatternNumberMuzS7.Visible=false;
                }
                if (CurrentPatternFromNumber.ShowType==NumberType.Deklination || CurrentPatternFromNumber.ShowType==NumberType.DeklinationWithGender) {
                    textBoxPatternNumberMuzM1.Text=CurrentPatternFromNumber.Shapes[7];
                    textBoxPatternNumberMuzM2.Text=CurrentPatternFromNumber.Shapes[8];
                    textBoxPatternNumberMuzM3.Text=CurrentPatternFromNumber.Shapes[9];
                    textBoxPatternNumberMuzM4.Text=CurrentPatternFromNumber.Shapes[10];
                    textBoxPatternNumberMuzM5.Text=CurrentPatternFromNumber.Shapes[11];
                    textBoxPatternNumberMuzM6.Text=CurrentPatternFromNumber.Shapes[12];
                    textBoxPatternNumberMuzM7.Text=CurrentPatternFromNumber.Shapes[13];   
                    textBoxPatternNumberMuzM1.Visible=true;
                    textBoxPatternNumberMuzM2.Visible=true;
                    textBoxPatternNumberMuzM3.Visible=true;
                    textBoxPatternNumberMuzM4.Visible=true;
                    textBoxPatternNumberMuzM5.Visible=true;
                    textBoxPatternNumberMuzM6.Visible=true;
                    textBoxPatternNumberMuzM7.Visible=true;
                    labelPatternNumberMuz.Visible=false;
                }else{ 
                    textBoxPatternNumberMuzM1.Visible=false;
                    textBoxPatternNumberMuzM2.Visible=false;
                    textBoxPatternNumberMuzM3.Visible=false;
                    textBoxPatternNumberMuzM4.Visible=false;
                    textBoxPatternNumberMuzM5.Visible=false;
                    textBoxPatternNumberMuzM6.Visible=false;
                    textBoxPatternNumberMuzM7.Visible=false;
                    labelPatternNumberMuz.Visible=true;
                }
                if (CurrentPatternFromNumber.ShowType==NumberType.DeklinationWithGender) {
                    textBoxPatternNumberMunS1.Text=CurrentPatternFromNumber.Shapes[14+0];
                    textBoxPatternNumberMunS2.Text=CurrentPatternFromNumber.Shapes[14+1];
                    textBoxPatternNumberMunS3.Text=CurrentPatternFromNumber.Shapes[14+2];
                    textBoxPatternNumberMunS4.Text=CurrentPatternFromNumber.Shapes[14+3];
                    textBoxPatternNumberMunS5.Text=CurrentPatternFromNumber.Shapes[14+4];
                    textBoxPatternNumberMunS6.Text=CurrentPatternFromNumber.Shapes[14+5];
                    textBoxPatternNumberMunS7.Text=CurrentPatternFromNumber.Shapes[14+6];
                    textBoxPatternNumberMunM1.Text=CurrentPatternFromNumber.Shapes[14+7];
                    textBoxPatternNumberMunM2.Text=CurrentPatternFromNumber.Shapes[14+8];
                    textBoxPatternNumberMunM3.Text=CurrentPatternFromNumber.Shapes[14+9];
                    textBoxPatternNumberMunM4.Text=CurrentPatternFromNumber.Shapes[14+10];
                    textBoxPatternNumberMunM5.Text=CurrentPatternFromNumber.Shapes[14+11];
                    textBoxPatternNumberMunM6.Text=CurrentPatternFromNumber.Shapes[14+12];
                    textBoxPatternNumberMunM7.Text=CurrentPatternFromNumber.Shapes[14+13];

                    textBoxPatternNumberZenS1.Text=CurrentPatternFromNumber.Shapes[28+0];
                    textBoxPatternNumberZenS2.Text=CurrentPatternFromNumber.Shapes[28+1];
                    textBoxPatternNumberZenS3.Text=CurrentPatternFromNumber.Shapes[28+2];
                    textBoxPatternNumberZenS4.Text=CurrentPatternFromNumber.Shapes[28+3];
                    textBoxPatternNumberZenS5.Text=CurrentPatternFromNumber.Shapes[28+4];
                    textBoxPatternNumberZenS6.Text=CurrentPatternFromNumber.Shapes[28+5];
                    textBoxPatternNumberZenS7.Text=CurrentPatternFromNumber.Shapes[28+6];
                    textBoxPatternNumberZenM1.Text=CurrentPatternFromNumber.Shapes[28+7];
                    textBoxPatternNumberZenM2.Text=CurrentPatternFromNumber.Shapes[28+8];
                    textBoxPatternNumberZenM3.Text=CurrentPatternFromNumber.Shapes[28+9];
                    textBoxPatternNumberZenM4.Text=CurrentPatternFromNumber.Shapes[28+10];
                    textBoxPatternNumberZenM5.Text=CurrentPatternFromNumber.Shapes[28+11];
                    textBoxPatternNumberZenM6.Text=CurrentPatternFromNumber.Shapes[28+12];
                    textBoxPatternNumberZenM7.Text=CurrentPatternFromNumber.Shapes[28+13];

                    textBoxPatternNumberStrS1.Text=CurrentPatternFromNumber.Shapes[42+0];
                    textBoxPatternNumberStrS2.Text=CurrentPatternFromNumber.Shapes[42+1];
                    textBoxPatternNumberStrS3.Text=CurrentPatternFromNumber.Shapes[42+2];
                    textBoxPatternNumberStrS4.Text=CurrentPatternFromNumber.Shapes[42+3];
                    textBoxPatternNumberStrS5.Text=CurrentPatternFromNumber.Shapes[42+4];
                    textBoxPatternNumberStrS6.Text=CurrentPatternFromNumber.Shapes[42+5];
                    textBoxPatternNumberStrS7.Text=CurrentPatternFromNumber.Shapes[42+6];
                    textBoxPatternNumberStrM1.Text=CurrentPatternFromNumber.Shapes[42+7];
                    textBoxPatternNumberStrM2.Text=CurrentPatternFromNumber.Shapes[42+8];
                    textBoxPatternNumberStrM3.Text=CurrentPatternFromNumber.Shapes[42+9];
                    textBoxPatternNumberStrM4.Text=CurrentPatternFromNumber.Shapes[42+10];
                    textBoxPatternNumberStrM5.Text=CurrentPatternFromNumber.Shapes[42+11];
                    textBoxPatternNumberStrM6.Text=CurrentPatternFromNumber.Shapes[42+12];
                    textBoxPatternNumberStrM7.Text=CurrentPatternFromNumber.Shapes[42+13];

                    textBoxPatternNumberMunS1.Visible=true;
                    textBoxPatternNumberMunS2.Visible=true;
                    textBoxPatternNumberMunS3.Visible=true;
                    textBoxPatternNumberMunS4.Visible=true;
                    textBoxPatternNumberMunS5.Visible=true;
                    textBoxPatternNumberMunS6.Visible=true;
                    textBoxPatternNumberMunS7.Visible=true;
                    textBoxPatternNumberMunM1.Visible=true;
                    textBoxPatternNumberMunM2.Visible=true;
                    textBoxPatternNumberMunM3.Visible=true;
                    textBoxPatternNumberMunM4.Visible=true;
                    textBoxPatternNumberMunM5.Visible=true;
                    textBoxPatternNumberMunM6.Visible=true;
                    textBoxPatternNumberMunM7.Visible=true;

                    textBoxPatternNumberZenS1.Visible=true;
                    textBoxPatternNumberZenS2.Visible=true;
                    textBoxPatternNumberZenS3.Visible=true;
                    textBoxPatternNumberZenS4.Visible=true;
                    textBoxPatternNumberZenS5.Visible=true;
                    textBoxPatternNumberZenS6.Visible=true;
                    textBoxPatternNumberZenS7.Visible=true;
                    textBoxPatternNumberZenM1.Visible=true;
                    textBoxPatternNumberZenM2.Visible=true;
                    textBoxPatternNumberZenM3.Visible=true;
                    textBoxPatternNumberZenM4.Visible=true;
                    textBoxPatternNumberZenM5.Visible=true;
                    textBoxPatternNumberZenM6.Visible=true;
                    textBoxPatternNumberZenM7.Visible=true;

                    textBoxPatternNumberStrS1.Visible=true;
                    textBoxPatternNumberStrS2.Visible=true;
                    textBoxPatternNumberStrS3.Visible=true;
                    textBoxPatternNumberStrS4.Visible=true;
                    textBoxPatternNumberStrS5.Visible=true;
                    textBoxPatternNumberStrS6.Visible=true;
                    textBoxPatternNumberStrS7.Visible=true;
                    textBoxPatternNumberStrM1.Visible=true;
                    textBoxPatternNumberStrM2.Visible=true;
                    textBoxPatternNumberStrM3.Visible=true;
                    textBoxPatternNumberStrM4.Visible=true;
                    textBoxPatternNumberStrM5.Visible=true;
                    textBoxPatternNumberStrM6.Visible=true;
                    textBoxPatternNumberStrM7.Visible=true;

                    labelPatternNumberMun.Visible=true;
                    labelPatternNumberMunFall.Visible=true;
                    labelPatternNumberMunSingle.Visible=true;
                    labelPatternNumberMunMultiple.Visible=true;

                    labelPatternNumberZen.Visible=true;
                    labelPatternNumberZenFall.Visible=true;
                    labelPatternNumberZenSingle.Visible=true;
                    labelPatternNumberZenMultiple.Visible=true;

                    labelPatternNumberStr.Visible=true;
                    labelPatternNumberStrFall.Visible=true;
                    labelPatternNumberStrSingle.Visible=true;
                    labelPatternNumberStrMultiple.Visible=true;

                    tableLayoutPanelPatternNumberStr.Visible=true;
                    tableLayoutPanelPatternNumberZen.Visible=true;
                    tableLayoutPanelPatternNumberMun.Visible=true;
                }else{ 
                    textBoxPatternNumberMunS1.Visible=false;
                    textBoxPatternNumberMunS2.Visible=false;
                    textBoxPatternNumberMunS3.Visible=false;
                    textBoxPatternNumberMunS4.Visible=false;
                    textBoxPatternNumberMunS5.Visible=false;
                    textBoxPatternNumberMunS6.Visible=false;
                    textBoxPatternNumberMunS7.Visible=false;
                    textBoxPatternNumberMunM1.Visible=false;
                    textBoxPatternNumberMunM2.Visible=false;
                    textBoxPatternNumberMunM3.Visible=false;
                    textBoxPatternNumberMunM4.Visible=false;
                    textBoxPatternNumberMunM5.Visible=false;
                    textBoxPatternNumberMunM6.Visible=false;
                    textBoxPatternNumberMunM7.Visible=false;

                    textBoxPatternNumberZenS1.Visible=false;
                    textBoxPatternNumberZenS2.Visible=false;
                    textBoxPatternNumberZenS3.Visible=false;
                    textBoxPatternNumberZenS4.Visible=false;
                    textBoxPatternNumberZenS5.Visible=false;
                    textBoxPatternNumberZenS6.Visible=false;
                    textBoxPatternNumberZenS7.Visible=false;
                    textBoxPatternNumberZenM1.Visible=false;
                    textBoxPatternNumberZenM2.Visible=false;
                    textBoxPatternNumberZenM3.Visible=false;
                    textBoxPatternNumberZenM4.Visible=false;
                    textBoxPatternNumberZenM5.Visible=false;
                    textBoxPatternNumberZenM6.Visible=false;
                    textBoxPatternNumberZenM7.Visible=false;

                    textBoxPatternNumberStrS1.Visible=false;
                    textBoxPatternNumberStrS2.Visible=false;
                    textBoxPatternNumberStrS3.Visible=false;
                    textBoxPatternNumberStrS4.Visible=false;
                    textBoxPatternNumberStrS5.Visible=false;
                    textBoxPatternNumberStrS6.Visible=false;
                    textBoxPatternNumberStrS7.Visible=false;
                    textBoxPatternNumberStrM1.Visible=false;
                    textBoxPatternNumberStrM2.Visible=false;
                    textBoxPatternNumberStrM3.Visible=false;
                    textBoxPatternNumberStrM4.Visible=false;
                    textBoxPatternNumberStrM5.Visible=false;
                    textBoxPatternNumberStrM6.Visible=false;
                    textBoxPatternNumberStrM7.Visible=false;

                    labelPatternNumberMun.Visible=false;
                    labelPatternNumberMunFall.Visible=false;
                    labelPatternNumberMunSingle.Visible=false;
                    labelPatternNumberMunMultiple.Visible=false;

                    labelPatternNumberZen.Visible=false;
                    labelPatternNumberZenFall.Visible=false;
                    labelPatternNumberZenSingle.Visible=false;
                    labelPatternNumberZenMultiple.Visible=false;

                    labelPatternNumberStr.Visible=false;
                    labelPatternNumberStrFall.Visible=false;
                    labelPatternNumberStrSingle.Visible=false;
                    labelPatternNumberStrMultiple.Visible=false;

                    
                    tableLayoutPanelPatternNumberStr.Visible=false;
                    tableLayoutPanelPatternNumberZen.Visible=false;
                    tableLayoutPanelPatternNumberMun.Visible=false;
                }
            }
     
            labelPatternNumberName.Visible=true;
            tableLayoutPanelPatternNumberMuz.Visible=true;
            labelPatternNumberType.Visible=true;
            comboBoxPatternNumberType.Visible=true;
        }
         
        void PatternNumberFromListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternNumbersFromFiltered.Count; indexCur++) {
                if (itemsPatternNumbersFrom[indexCur]==CurrentPatternFromNumber) { 
                    int indexList=PatternNumberFromlistBox.SelectedIndex;
                    if (indexList==indexCur) return;
                    PatternNumberFromlistBox.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void PatternNumberFromSaveCurrent() {
            if (CurrentPatternFromNumber==null) return;
            CurrentPatternFromNumber.ShowType=(NumberType)comboBoxPatternNumberType.SelectedIndex;
            CurrentPatternFromNumber.Name=textBoxPatternNumberName.Text;

            if (CurrentPatternFromNumber.ShowType!=NumberType.Unknown) {
                if (CurrentPatternFromNumber.ShowType==NumberType.NoDeklination || CurrentPatternFromNumber.ShowType==NumberType.Deklination || CurrentPatternFromNumber.ShowType==NumberType.DeklinationOnlySingle || CurrentPatternFromNumber.ShowType==NumberType.DeklinationWithGender) {
                    CurrentPatternFromNumber.Shapes[0]=textBoxPatternNumberFromMuzS1.Text;
                }
                if (CurrentPatternFromNumber.ShowType==NumberType.Deklination || CurrentPatternFromNumber.ShowType==NumberType.DeklinationOnlySingle || CurrentPatternFromNumber.ShowType==NumberType.DeklinationWithGender) {
                    CurrentPatternFromNumber.Shapes[1]=textBoxPatternNumberFromMuzS2.Text;
                    CurrentPatternFromNumber.Shapes[2]=textBoxPatternNumberFromMuzS3.Text;
                    CurrentPatternFromNumber.Shapes[3]=textBoxPatternNumberMuzS4.Text;
                    CurrentPatternFromNumber.Shapes[4]=textBoxPatternNumberMuzS5.Text;
                    CurrentPatternFromNumber.Shapes[5]=textBoxPatternNumberMuzS6.Text;
                    CurrentPatternFromNumber.Shapes[6]=textBoxPatternNumberMuzS7.Text;
                }
                if (CurrentPatternFromNumber.ShowType==NumberType.Deklination || CurrentPatternFromNumber.ShowType==NumberType.DeklinationWithGender) {
                    CurrentPatternFromNumber.Shapes[7]=textBoxPatternNumberMuzM1.Text;
                    CurrentPatternFromNumber.Shapes[8]=textBoxPatternNumberMuzM2.Text;
                    CurrentPatternFromNumber.Shapes[9]=textBoxPatternNumberMuzM3.Text;
                    CurrentPatternFromNumber.Shapes[10]=textBoxPatternNumberMuzM4.Text;
                    CurrentPatternFromNumber.Shapes[11]=textBoxPatternNumberMuzM5.Text;
                    CurrentPatternFromNumber.Shapes[12]=textBoxPatternNumberMuzM6.Text;
                    CurrentPatternFromNumber.Shapes[13]=textBoxPatternNumberMuzM7.Text;
                }
                if (CurrentPatternFromNumber.ShowType==NumberType.DeklinationWithGender) {
                    CurrentPatternFromNumber.Shapes[14+0] =textBoxPatternNumberMunS1.Text;
                    CurrentPatternFromNumber.Shapes[14+1] =textBoxPatternNumberMunS2.Text;
                    CurrentPatternFromNumber.Shapes[14+2] =textBoxPatternNumberMunS3.Text;
                    CurrentPatternFromNumber.Shapes[14+3] =textBoxPatternNumberMunS4.Text;
                    CurrentPatternFromNumber.Shapes[14+4] =textBoxPatternNumberMunS5.Text;
                    CurrentPatternFromNumber.Shapes[14+5] =textBoxPatternNumberMunS6.Text;
                    CurrentPatternFromNumber.Shapes[14+6] =textBoxPatternNumberMunS7.Text;
                    CurrentPatternFromNumber.Shapes[14+7] =textBoxPatternNumberMunM1.Text;
                    CurrentPatternFromNumber.Shapes[14+8] =textBoxPatternNumberMunM2.Text;
                    CurrentPatternFromNumber.Shapes[14+9] =textBoxPatternNumberMunM3.Text;
                    CurrentPatternFromNumber.Shapes[14+10]=textBoxPatternNumberMunM4.Text;
                    CurrentPatternFromNumber.Shapes[14+11]=textBoxPatternNumberMunM5.Text;
                    CurrentPatternFromNumber.Shapes[14+12]=textBoxPatternNumberMunM6.Text;
                    CurrentPatternFromNumber.Shapes[14+13]=textBoxPatternNumberMunM7.Text;

                    CurrentPatternFromNumber.Shapes[28+0] =textBoxPatternNumberZenS1.Text;
                    CurrentPatternFromNumber.Shapes[28+1] =textBoxPatternNumberZenS2.Text;
                    CurrentPatternFromNumber.Shapes[28+2] =textBoxPatternNumberZenS3.Text;
                    CurrentPatternFromNumber.Shapes[28+3] =textBoxPatternNumberZenS4.Text;
                    CurrentPatternFromNumber.Shapes[28+4] =textBoxPatternNumberZenS5.Text;
                    CurrentPatternFromNumber.Shapes[28+5] =textBoxPatternNumberZenS6.Text;
                    CurrentPatternFromNumber.Shapes[28+6] =textBoxPatternNumberZenS7.Text;
                    CurrentPatternFromNumber.Shapes[28+7] =textBoxPatternNumberZenM1.Text;
                    CurrentPatternFromNumber.Shapes[28+8] =textBoxPatternNumberZenM2.Text;
                    CurrentPatternFromNumber.Shapes[28+9] =textBoxPatternNumberZenM3.Text;
                    CurrentPatternFromNumber.Shapes[28+10]=textBoxPatternNumberZenM4.Text;
                    CurrentPatternFromNumber.Shapes[28+11]=textBoxPatternNumberZenM5.Text;
                    CurrentPatternFromNumber.Shapes[28+12]=textBoxPatternNumberZenM6.Text;
                    CurrentPatternFromNumber.Shapes[28+13]=textBoxPatternNumberZenM7.Text;

                    CurrentPatternFromNumber.Shapes[42+0] =textBoxPatternNumberStrS1.Text;
                    CurrentPatternFromNumber.Shapes[42+1] =textBoxPatternNumberStrS2.Text;
                    CurrentPatternFromNumber.Shapes[42+2] =textBoxPatternNumberStrS3.Text;
                    CurrentPatternFromNumber.Shapes[42+3] =textBoxPatternNumberStrS4.Text;
                    CurrentPatternFromNumber.Shapes[42+4] =textBoxPatternNumberStrS5.Text;
                    CurrentPatternFromNumber.Shapes[42+5] =textBoxPatternNumberStrS6.Text;
                    CurrentPatternFromNumber.Shapes[42+6] =textBoxPatternNumberStrS7.Text;
                    CurrentPatternFromNumber.Shapes[42+7] =textBoxPatternNumberStrM1.Text;
                    CurrentPatternFromNumber.Shapes[42+8] =textBoxPatternNumberStrM2.Text;
                    CurrentPatternFromNumber.Shapes[42+9] =textBoxPatternNumberStrM3.Text;
                    CurrentPatternFromNumber.Shapes[42+10]=textBoxPatternNumberStrM4.Text;
                    CurrentPatternFromNumber.Shapes[42+11]=textBoxPatternNumberStrM5.Text;
                    CurrentPatternFromNumber.Shapes[42+12]=textBoxPatternNumberStrM6.Text;
                    CurrentPatternFromNumber.Shapes[42+13]=textBoxPatternNumberStrM7.Text;
                }
            }
        } 
              
        void PatternNumberFromSetNone(){ 
            textBoxPatternNumberName.Text="";

            textBoxPatternNumberFromMuzS1.Text="";
            textBoxPatternNumberFromMuzS2.Text="";
            textBoxPatternNumberFromMuzS3.Text="";
            textBoxPatternNumberMuzS4.Text="";
            textBoxPatternNumberMuzS5.Text="";
            textBoxPatternNumberMuzS6.Text="";
            textBoxPatternNumberMuzS7.Text="";
            textBoxPatternNumberMuzM1.Text="";
            textBoxPatternNumberMuzM2.Text="";
            textBoxPatternNumberMuzM3.Text="";
            textBoxPatternNumberMuzM4.Text="";
            textBoxPatternNumberMuzM5.Text="";
            textBoxPatternNumberMuzM6.Text="";
            textBoxPatternNumberMuzM7.Text="";

            textBoxPatternNumberMunS1.Text="";
            textBoxPatternNumberMunS2.Text="";
            textBoxPatternNumberMunS3.Text="";
            textBoxPatternNumberMunS4.Text="";
            textBoxPatternNumberMunS5.Text="";
            textBoxPatternNumberMunS6.Text="";
            textBoxPatternNumberMunS7.Text="";
            textBoxPatternNumberMunM1.Text="";
            textBoxPatternNumberMunM2.Text="";
            textBoxPatternNumberMunM3.Text="";
            textBoxPatternNumberMunM4.Text="";
            textBoxPatternNumberMunM5.Text="";
            textBoxPatternNumberMunM6.Text="";
            textBoxPatternNumberMunM7.Text="";

            textBoxPatternNumberZenS1.Text="";
            textBoxPatternNumberZenS2.Text="";
            textBoxPatternNumberZenS3.Text="";
            textBoxPatternNumberZenS4.Text="";
            textBoxPatternNumberZenS5.Text="";
            textBoxPatternNumberZenS6.Text="";
            textBoxPatternNumberZenS7.Text="";
            textBoxPatternNumberZenM1.Text="";
            textBoxPatternNumberZenM2.Text="";
            textBoxPatternNumberZenM3.Text="";
            textBoxPatternNumberZenM4.Text="";
            textBoxPatternNumberZenM5.Text="";
            textBoxPatternNumberZenM6.Text="";
            textBoxPatternNumberZenM7.Text="";

            textBoxPatternNumberStrS1.Text="";
            textBoxPatternNumberStrS2.Text="";
            textBoxPatternNumberStrS3.Text="";
            textBoxPatternNumberStrS4.Text="";
            textBoxPatternNumberStrS5.Text="";
            textBoxPatternNumberStrS6.Text="";
            textBoxPatternNumberStrS7.Text="";
            textBoxPatternNumberStrM1.Text="";
            textBoxPatternNumberStrM2.Text="";
            textBoxPatternNumberStrM3.Text="";
            textBoxPatternNumberStrM4.Text="";
            textBoxPatternNumberStrM5.Text="";
            textBoxPatternNumberStrM6.Text="";
            textBoxPatternNumberStrM7.Text="";

            labelPatternNumberName.Visible=false;
            tableLayoutPanelPatternNumberMuz.Visible=false;
            labelPatternNumberMuzFall.Visible=false;
            labelPatternNumberMuzMultiple.Visible=false;
            labelPatternNumberMuzSingle.Visible=false;
            labelPatternNumberType.Visible=false;
            comboBoxPatternNumberType.Visible=false;

            textBoxPatternNumberFromMuzS1.Visible=false;
            textBoxPatternNumberFromMuzS2.Visible=false;
            textBoxPatternNumberFromMuzS3.Visible=false;
            textBoxPatternNumberMuzS4.Visible=false;
            textBoxPatternNumberMuzS5.Visible=false;
            textBoxPatternNumberMuzS6.Visible=false;
            textBoxPatternNumberMuzS7.Visible=false;
            textBoxPatternNumberMuzM1.Visible=false;
            textBoxPatternNumberMuzM2.Visible=false;
            textBoxPatternNumberMuzM3.Visible=false;
            textBoxPatternNumberMuzM4.Visible=false;
            textBoxPatternNumberMuzM5.Visible=false;
            textBoxPatternNumberMuzM6.Visible=false;
            textBoxPatternNumberMuzM7.Visible=false;

            
            textBoxPatternNumberMunS1.Visible=false;
            textBoxPatternNumberMunS2.Visible=false;
            textBoxPatternNumberMunS3.Visible=false;
            textBoxPatternNumberMunS4.Visible=false;
            textBoxPatternNumberMunS5.Visible=false;
            textBoxPatternNumberMunS6.Visible=false;
            textBoxPatternNumberMunS7.Visible=false;
            textBoxPatternNumberMunM1.Visible=false;
            textBoxPatternNumberMunM2.Visible=false;
            textBoxPatternNumberMunM3.Visible=false;
            textBoxPatternNumberMunM4.Visible=false;
            textBoxPatternNumberMunM5.Visible=false;
            textBoxPatternNumberMunM6.Visible=false;
            textBoxPatternNumberMunM7.Visible=false;

            
            textBoxPatternNumberZenS1.Visible=false;
            textBoxPatternNumberZenS2.Visible=false;
            textBoxPatternNumberZenS3.Visible=false;
            textBoxPatternNumberZenS4.Visible=false;
            textBoxPatternNumberZenS5.Visible=false;
            textBoxPatternNumberZenS6.Visible=false;
            textBoxPatternNumberZenS7.Visible=false;
            textBoxPatternNumberZenM1.Visible=false;
            textBoxPatternNumberZenM2.Visible=false;
            textBoxPatternNumberZenM3.Visible=false;
            textBoxPatternNumberZenM4.Visible=false;
            textBoxPatternNumberZenM5.Visible=false;
            textBoxPatternNumberZenM6.Visible=false;
            textBoxPatternNumberZenM7.Visible=false;

            
            textBoxPatternNumberStrS1.Visible=false;
            textBoxPatternNumberStrS2.Visible=false;
            textBoxPatternNumberStrS3.Visible=false;
            textBoxPatternNumberStrS4.Visible=false;
            textBoxPatternNumberStrS5.Visible=false;
            textBoxPatternNumberStrS6.Visible=false;
            textBoxPatternNumberStrS7.Visible=false;
            textBoxPatternNumberStrM1.Visible=false;
            textBoxPatternNumberStrM2.Visible=false;
            textBoxPatternNumberStrM3.Visible=false;
            textBoxPatternNumberStrM4.Visible=false;
            textBoxPatternNumberStrM5.Visible=false;
            textBoxPatternNumberStrM6.Visible=false;
            textBoxPatternNumberStrM7.Visible=false;

            textBoxPatternNumberName.Visible=false;
        }
        
        void PatternNumberFromClear() { 
            PatternNumberFromlistBox.Items.Clear();
            PatternNumberFromSetNone();
            itemsPatternNumbersFromFiltered?.Clear();
            itemsPatternNumbersFrom?.Clear();
            CurrentPatternFromNumber=null;
        }
        #endregion

        #region NumberPattern To
        void PatternNumberToListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternNumberToSaveCurrent();
            
            int index=PatternNumberTolistBox.SelectedIndex;
            if (itemsPatternNumbersTo.Count==0) {
                PatternNumberToSetNone();
                return;
            }
            if (index>=itemsPatternNumbersTo.Count) 
                index=itemsPatternNumbersTo.Count-1;
            if (index<0)
                index=0;
           
            CurrentPatternToNumber=itemsPatternNumbersTo[index];
            PatternNumberToSetCurrent();
            PatternNumberToSetListBox();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void PatternNumberToButtonAdd_Click(object sender, EventArgs e) {
            PatternNumberToAddNewItem();
        }

        void PatternNumberToButtonRemove_Click(object sender, EventArgs e) {
            PatternNumberToRemoveItem(CurrentPatternToNumber);
            PatternNumberToTextBoxFilter_TextChanged(null, new EventArgs());
        }
        
        void PatternNumberToTextBoxFilter_TextChanged(object sender, EventArgs e) {
            PatternNumberToSaveCurrent();

            // Získej aktuální prvek
            ItemPatternNumber selectedId=null;
            if (PatternNumberTolistBox.SelectedIndex!=-1) {
                selectedId=itemsPatternNumbersToFiltered[PatternNumberTolistBox.SelectedIndex];
            }
            
            PatternNumberToRefreshFilteredList();

            PatternNumberTolistBox.Items.Clear();
            for (int i=0; i<itemsPatternNumbersToFiltered.Count; i++) {
                ItemPatternNumber item = itemsPatternNumbersToFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                PatternNumberTolistBox.Items.Add(textToAdd);
            }

            //SetListBoxNumber();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsPatternNumbersToFiltered.Count; i++){
                    if (itemsPatternNumbersToFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    PatternNumberTolistBox.SelectedIndex=-1;
                    CurrentPatternToNumber=null;
                } else PatternNumberTolistBox.SelectedIndex=outIndex;
            } else {
                PatternNumberTolistBox.SelectedIndex=-1;
                CurrentPatternToNumber=null;
            }
            PatternNumberToSetCurrent();
        }
            
        void PatternNumberToRemoveCurrent(object sender, EventArgs e) {
            itemsPatternNumbersTo.Remove(CurrentPatternToNumber);
        }

        void PatternNumberToSetListBox() { 
            string filter=textBoxPatternNumberFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=PatternNumberTolistBox.SelectedIndex;
            PatternNumberTolistBox.Items.Clear();
            for (int i=0; i<itemsPatternNumbersToFiltered.Count; i++) {
                ItemPatternNumber item = itemsPatternNumbersToFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                PatternNumberTolistBox.Items.Add(textToAdd);
            }

            if (index>=PatternNumberTolistBox.Items.Count)index=PatternNumberTolistBox.Items.Count-1;
            PatternNumberTolistBox.SelectedIndex=index;
        }

        void PatternNumberToRefreshFilteredList() {
            if (itemsPatternNumbersToFiltered==null) itemsPatternNumbersToFiltered=new List<ItemPatternNumber>();
            itemsPatternNumbersToFiltered.Clear();
            string filter=textBoxPatternNumberFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsPatternNumbersTo.Count; i++) {
                    ItemPatternNumber item = itemsPatternNumbersTo[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsPatternNumbersToFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsPatternNumbersTo.Count; i++) {
                    ItemPatternNumber item = itemsPatternNumbersTo[i];
                    itemsPatternNumbersToFiltered.Add(item);
                }
            }
        }
               
        void PatternNumberToAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternNumberToSaveCurrent();

            var newItem=new ItemPatternNumber();
           // newItem.ID=itemsNumbers.Count;
            itemsPatternNumbersTo.Add(newItem);
            CurrentPatternToNumber=newItem;
            PatternNumberToRefreshFilteredList();
            PatternNumberToSetListBox(); 
            PatternNumberToListBoxSetCurrent();
            PatternNumberToSetCurrent(); 
            
            doingJob=false;
        }
   
        void PatternNumberToRemoveItem(ItemPatternNumber item) { 
            Edited=true;
            ChangeCaptionText();
            itemsPatternNumbersTo.Remove(item);
            PatternNumberToRefreshFilteredList();
            PatternNumberToSetListBox();
            PatternNumberToSetCurrent();
        } 
           
        void PatternNumberToSetCurrent(){
            if (itemsPatternNumbersToFiltered.Count==0) {
                PatternNumberToSetNone();
                return;
            }

            int index=PatternNumberTolistBox.SelectedIndex;
            if (index>=itemsPatternNumbersToFiltered.Count) index=itemsPatternNumbersToFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternToNumber=itemsPatternNumbersToFiltered[index];
            textBoxPatternNumberName.Text=CurrentPatternToNumber.Name;
            comboBoxPatternNumberType.SelectedIndex=(int)CurrentPatternToNumber.ShowType;
            
            if (CurrentPatternToNumber.ShowType!=NumberType.Unknown) {
                if (CurrentPatternToNumber.ShowType==NumberType.NoDeklination || CurrentPatternToNumber.ShowType==NumberType.Deklination || CurrentPatternToNumber.ShowType==NumberType.DeklinationOnlySingle || CurrentPatternToNumber.ShowType==NumberType.DeklinationWithGender) {
                    textBoxPatternNumberFromMuzS1.Text=CurrentPatternToNumber.Shapes[0];
                    textBoxPatternNumberFromMuzS1.Visible=true;       
                    
                    textBoxPatternNumberName.Visible=true;
                    labelPatternNumberMuzFall.Visible=true;
                    labelPatternNumberMuzMultiple.Visible=true;
                    labelPatternNumberMuzSingle.Visible=true;
                } else { 
                    textBoxPatternNumberFromMuzS1.Visible=false;
                    textBoxPatternNumberName.Visible=false;
                    labelPatternNumberMuzFall.Visible=false;
                    labelPatternNumberMuzMultiple.Visible=false;
                    labelPatternNumberMuzSingle.Visible=false;
                }
                if (CurrentPatternToNumber.ShowType==NumberType.Deklination || CurrentPatternToNumber.ShowType==NumberType.DeklinationOnlySingle || CurrentPatternToNumber.ShowType==NumberType.DeklinationWithGender) {
                    textBoxPatternNumberFromMuzS2.Text=CurrentPatternToNumber.Shapes[1];
                    textBoxPatternNumberFromMuzS3.Text=CurrentPatternToNumber.Shapes[2];
                    textBoxPatternNumberMuzS4.Text=CurrentPatternToNumber.Shapes[3];
                    textBoxPatternNumberMuzS5.Text=CurrentPatternToNumber.Shapes[4];
                    textBoxPatternNumberMuzS6.Text=CurrentPatternToNumber.Shapes[5];
                    textBoxPatternNumberMuzS7.Text=CurrentPatternToNumber.Shapes[6];
                    textBoxPatternNumberFromMuzS2.Visible=true;
                    textBoxPatternNumberFromMuzS3.Visible=true;
                    textBoxPatternNumberMuzS4.Visible=true;
                    textBoxPatternNumberMuzS5.Visible=true;
                    textBoxPatternNumberMuzS6.Visible=true;
                    textBoxPatternNumberMuzS7.Visible=true;
                } else { 
                    textBoxPatternNumberFromMuzS2.Visible=false;
                    textBoxPatternNumberFromMuzS3.Visible=false;
                    textBoxPatternNumberMuzS4.Visible=false;
                    textBoxPatternNumberMuzS5.Visible=false;
                    textBoxPatternNumberMuzS6.Visible=false;
                    textBoxPatternNumberMuzS7.Visible=false;
                }
                if (CurrentPatternToNumber.ShowType==NumberType.Deklination || CurrentPatternToNumber.ShowType==NumberType.DeklinationWithGender) {
                    textBoxPatternNumberMuzM1.Text=CurrentPatternToNumber.Shapes[7];
                    textBoxPatternNumberMuzM2.Text=CurrentPatternToNumber.Shapes[8];
                    textBoxPatternNumberMuzM3.Text=CurrentPatternToNumber.Shapes[9];
                    textBoxPatternNumberMuzM4.Text=CurrentPatternToNumber.Shapes[10];
                    textBoxPatternNumberMuzM5.Text=CurrentPatternToNumber.Shapes[11];
                    textBoxPatternNumberMuzM6.Text=CurrentPatternToNumber.Shapes[12];
                    textBoxPatternNumberMuzM7.Text=CurrentPatternToNumber.Shapes[13];   
                    textBoxPatternNumberMuzM1.Visible=true;
                    textBoxPatternNumberMuzM2.Visible=true;
                    textBoxPatternNumberMuzM3.Visible=true;
                    textBoxPatternNumberMuzM4.Visible=true;
                    textBoxPatternNumberMuzM5.Visible=true;
                    textBoxPatternNumberMuzM6.Visible=true;
                    textBoxPatternNumberMuzM7.Visible=true;
                    labelPatternNumberMuz.Visible=false;
                }else{ 
                    textBoxPatternNumberMuzM1.Visible=false;
                    textBoxPatternNumberMuzM2.Visible=false;
                    textBoxPatternNumberMuzM3.Visible=false;
                    textBoxPatternNumberMuzM4.Visible=false;
                    textBoxPatternNumberMuzM5.Visible=false;
                    textBoxPatternNumberMuzM6.Visible=false;
                    textBoxPatternNumberMuzM7.Visible=false;
                    labelPatternNumberMuz.Visible=true;
                }
                if (CurrentPatternToNumber.ShowType==NumberType.DeklinationWithGender) {
                    textBoxPatternNumberMunS1.Text=CurrentPatternToNumber.Shapes[14+0];
                    textBoxPatternNumberMunS2.Text=CurrentPatternToNumber.Shapes[14+1];
                    textBoxPatternNumberMunS3.Text=CurrentPatternToNumber.Shapes[14+2];
                    textBoxPatternNumberMunS4.Text=CurrentPatternToNumber.Shapes[14+3];
                    textBoxPatternNumberMunS5.Text=CurrentPatternToNumber.Shapes[14+4];
                    textBoxPatternNumberMunS6.Text=CurrentPatternToNumber.Shapes[14+5];
                    textBoxPatternNumberMunS7.Text=CurrentPatternToNumber.Shapes[14+6];
                    textBoxPatternNumberMunM1.Text=CurrentPatternToNumber.Shapes[14+7];
                    textBoxPatternNumberMunM2.Text=CurrentPatternToNumber.Shapes[14+8];
                    textBoxPatternNumberMunM3.Text=CurrentPatternToNumber.Shapes[14+9];
                    textBoxPatternNumberMunM4.Text=CurrentPatternToNumber.Shapes[14+10];
                    textBoxPatternNumberMunM5.Text=CurrentPatternToNumber.Shapes[14+11];
                    textBoxPatternNumberMunM6.Text=CurrentPatternToNumber.Shapes[14+12];
                    textBoxPatternNumberMunM7.Text=CurrentPatternToNumber.Shapes[14+13];

                    textBoxPatternNumberZenS1.Text=CurrentPatternToNumber.Shapes[28+0];
                    textBoxPatternNumberZenS2.Text=CurrentPatternToNumber.Shapes[28+1];
                    textBoxPatternNumberZenS3.Text=CurrentPatternToNumber.Shapes[28+2];
                    textBoxPatternNumberZenS4.Text=CurrentPatternToNumber.Shapes[28+3];
                    textBoxPatternNumberZenS5.Text=CurrentPatternToNumber.Shapes[28+4];
                    textBoxPatternNumberZenS6.Text=CurrentPatternToNumber.Shapes[28+5];
                    textBoxPatternNumberZenS7.Text=CurrentPatternToNumber.Shapes[28+6];
                    textBoxPatternNumberZenM1.Text=CurrentPatternToNumber.Shapes[28+7];
                    textBoxPatternNumberZenM2.Text=CurrentPatternToNumber.Shapes[28+8];
                    textBoxPatternNumberZenM3.Text=CurrentPatternToNumber.Shapes[28+9];
                    textBoxPatternNumberZenM4.Text=CurrentPatternToNumber.Shapes[28+10];
                    textBoxPatternNumberZenM5.Text=CurrentPatternToNumber.Shapes[28+11];
                    textBoxPatternNumberZenM6.Text=CurrentPatternToNumber.Shapes[28+12];
                    textBoxPatternNumberZenM7.Text=CurrentPatternToNumber.Shapes[28+13];

                    textBoxPatternNumberStrS1.Text=CurrentPatternToNumber.Shapes[42+0];
                    textBoxPatternNumberStrS2.Text=CurrentPatternToNumber.Shapes[42+1];
                    textBoxPatternNumberStrS3.Text=CurrentPatternToNumber.Shapes[42+2];
                    textBoxPatternNumberStrS4.Text=CurrentPatternToNumber.Shapes[42+3];
                    textBoxPatternNumberStrS5.Text=CurrentPatternToNumber.Shapes[42+4];
                    textBoxPatternNumberStrS6.Text=CurrentPatternToNumber.Shapes[42+5];
                    textBoxPatternNumberStrS7.Text=CurrentPatternToNumber.Shapes[42+6];
                    textBoxPatternNumberStrM1.Text=CurrentPatternToNumber.Shapes[42+7];
                    textBoxPatternNumberStrM2.Text=CurrentPatternToNumber.Shapes[42+8];
                    textBoxPatternNumberStrM3.Text=CurrentPatternToNumber.Shapes[42+9];
                    textBoxPatternNumberStrM4.Text=CurrentPatternToNumber.Shapes[42+10];
                    textBoxPatternNumberStrM5.Text=CurrentPatternToNumber.Shapes[42+11];
                    textBoxPatternNumberStrM6.Text=CurrentPatternToNumber.Shapes[42+12];
                    textBoxPatternNumberStrM7.Text=CurrentPatternToNumber.Shapes[42+13];

                    textBoxPatternNumberMunS1.Visible=true;
                    textBoxPatternNumberMunS2.Visible=true;
                    textBoxPatternNumberMunS3.Visible=true;
                    textBoxPatternNumberMunS4.Visible=true;
                    textBoxPatternNumberMunS5.Visible=true;
                    textBoxPatternNumberMunS6.Visible=true;
                    textBoxPatternNumberMunS7.Visible=true;
                    textBoxPatternNumberMunM1.Visible=true;
                    textBoxPatternNumberMunM2.Visible=true;
                    textBoxPatternNumberMunM3.Visible=true;
                    textBoxPatternNumberMunM4.Visible=true;
                    textBoxPatternNumberMunM5.Visible=true;
                    textBoxPatternNumberMunM6.Visible=true;
                    textBoxPatternNumberMunM7.Visible=true;

                    textBoxPatternNumberZenS1.Visible=true;
                    textBoxPatternNumberZenS2.Visible=true;
                    textBoxPatternNumberZenS3.Visible=true;
                    textBoxPatternNumberZenS4.Visible=true;
                    textBoxPatternNumberZenS5.Visible=true;
                    textBoxPatternNumberZenS6.Visible=true;
                    textBoxPatternNumberZenS7.Visible=true;
                    textBoxPatternNumberZenM1.Visible=true;
                    textBoxPatternNumberZenM2.Visible=true;
                    textBoxPatternNumberZenM3.Visible=true;
                    textBoxPatternNumberZenM4.Visible=true;
                    textBoxPatternNumberZenM5.Visible=true;
                    textBoxPatternNumberZenM6.Visible=true;
                    textBoxPatternNumberZenM7.Visible=true;

                    textBoxPatternNumberStrS1.Visible=true;
                    textBoxPatternNumberStrS2.Visible=true;
                    textBoxPatternNumberStrS3.Visible=true;
                    textBoxPatternNumberStrS4.Visible=true;
                    textBoxPatternNumberStrS5.Visible=true;
                    textBoxPatternNumberStrS6.Visible=true;
                    textBoxPatternNumberStrS7.Visible=true;
                    textBoxPatternNumberStrM1.Visible=true;
                    textBoxPatternNumberStrM2.Visible=true;
                    textBoxPatternNumberStrM3.Visible=true;
                    textBoxPatternNumberStrM4.Visible=true;
                    textBoxPatternNumberStrM5.Visible=true;
                    textBoxPatternNumberStrM6.Visible=true;
                    textBoxPatternNumberStrM7.Visible=true;

                    labelPatternNumberMun.Visible=true;
                    labelPatternNumberMunFall.Visible=true;
                    labelPatternNumberMunSingle.Visible=true;
                    labelPatternNumberMunMultiple.Visible=true;

                    labelPatternNumberZen.Visible=true;
                    labelPatternNumberZenFall.Visible=true;
                    labelPatternNumberZenSingle.Visible=true;
                    labelPatternNumberZenMultiple.Visible=true;

                    labelPatternNumberStr.Visible=true;
                    labelPatternNumberStrFall.Visible=true;
                    labelPatternNumberStrSingle.Visible=true;
                    labelPatternNumberStrMultiple.Visible=true;

                    tableLayoutPanelPatternNumberStr.Visible=true;
                    tableLayoutPanelPatternNumberZen.Visible=true;
                    tableLayoutPanelPatternNumberMun.Visible=true;
                }else{ 
                    textBoxPatternNumberMunS1.Visible=false;
                    textBoxPatternNumberMunS2.Visible=false;
                    textBoxPatternNumberMunS3.Visible=false;
                    textBoxPatternNumberMunS4.Visible=false;
                    textBoxPatternNumberMunS5.Visible=false;
                    textBoxPatternNumberMunS6.Visible=false;
                    textBoxPatternNumberMunS7.Visible=false;
                    textBoxPatternNumberMunM1.Visible=false;
                    textBoxPatternNumberMunM2.Visible=false;
                    textBoxPatternNumberMunM3.Visible=false;
                    textBoxPatternNumberMunM4.Visible=false;
                    textBoxPatternNumberMunM5.Visible=false;
                    textBoxPatternNumberMunM6.Visible=false;
                    textBoxPatternNumberMunM7.Visible=false;

                    textBoxPatternNumberZenS1.Visible=false;
                    textBoxPatternNumberZenS2.Visible=false;
                    textBoxPatternNumberZenS3.Visible=false;
                    textBoxPatternNumberZenS4.Visible=false;
                    textBoxPatternNumberZenS5.Visible=false;
                    textBoxPatternNumberZenS6.Visible=false;
                    textBoxPatternNumberZenS7.Visible=false;
                    textBoxPatternNumberZenM1.Visible=false;
                    textBoxPatternNumberZenM2.Visible=false;
                    textBoxPatternNumberZenM3.Visible=false;
                    textBoxPatternNumberZenM4.Visible=false;
                    textBoxPatternNumberZenM5.Visible=false;
                    textBoxPatternNumberZenM6.Visible=false;
                    textBoxPatternNumberZenM7.Visible=false;

                    textBoxPatternNumberStrS1.Visible=false;
                    textBoxPatternNumberStrS2.Visible=false;
                    textBoxPatternNumberStrS3.Visible=false;
                    textBoxPatternNumberStrS4.Visible=false;
                    textBoxPatternNumberStrS5.Visible=false;
                    textBoxPatternNumberStrS6.Visible=false;
                    textBoxPatternNumberStrS7.Visible=false;
                    textBoxPatternNumberStrM1.Visible=false;
                    textBoxPatternNumberStrM2.Visible=false;
                    textBoxPatternNumberStrM3.Visible=false;
                    textBoxPatternNumberStrM4.Visible=false;
                    textBoxPatternNumberStrM5.Visible=false;
                    textBoxPatternNumberStrM6.Visible=false;
                    textBoxPatternNumberStrM7.Visible=false;

                    labelPatternNumberMun.Visible=false;
                    labelPatternNumberMunFall.Visible=false;
                    labelPatternNumberMunSingle.Visible=false;
                    labelPatternNumberMunMultiple.Visible=false;

                    labelPatternNumberZen.Visible=false;
                    labelPatternNumberZenFall.Visible=false;
                    labelPatternNumberZenSingle.Visible=false;
                    labelPatternNumberZenMultiple.Visible=false;

                    labelPatternNumberStr.Visible=false;
                    labelPatternNumberStrFall.Visible=false;
                    labelPatternNumberStrSingle.Visible=false;
                    labelPatternNumberStrMultiple.Visible=false;

                    
                    tableLayoutPanelPatternNumberStr.Visible=false;
                    tableLayoutPanelPatternNumberZen.Visible=false;
                    tableLayoutPanelPatternNumberMun.Visible=false;
                }
            }
     
            labelPatternNumberName.Visible=true;
            tableLayoutPanelPatternNumberMuz.Visible=true;
            labelPatternNumberType.Visible=true;
            comboBoxPatternNumberType.Visible=true;
        }
         
        void PatternNumberToListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternNumbersToFiltered.Count; indexCur++) {
                if (itemsPatternNumbersTo[indexCur]==CurrentPatternToNumber) { 
                    int indexList=PatternNumberTolistBox.SelectedIndex;
                    if (indexList==indexCur) return;
                    PatternNumberTolistBox.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void PatternNumberToSaveCurrent() {
            if (CurrentPatternToNumber==null) return;
            CurrentPatternToNumber.ShowType=(NumberType)comboBoxPatternNumberType.SelectedIndex;
            CurrentPatternToNumber.Name=textBoxPatternNumberName.Text;

            if (CurrentPatternToNumber.ShowType!=NumberType.Unknown) {
                if (CurrentPatternToNumber.ShowType==NumberType.NoDeklination || CurrentPatternToNumber.ShowType==NumberType.Deklination || CurrentPatternToNumber.ShowType==NumberType.DeklinationOnlySingle || CurrentPatternToNumber.ShowType==NumberType.DeklinationWithGender) {
                    CurrentPatternToNumber.Shapes[0]=textBoxPatternNumberFromMuzS1.Text;
                }
                if (CurrentPatternToNumber.ShowType==NumberType.Deklination || CurrentPatternToNumber.ShowType==NumberType.DeklinationOnlySingle || CurrentPatternToNumber.ShowType==NumberType.DeklinationWithGender) {
                    CurrentPatternToNumber.Shapes[1]=textBoxPatternNumberFromMuzS2.Text;
                    CurrentPatternToNumber.Shapes[2]=textBoxPatternNumberFromMuzS3.Text;
                    CurrentPatternToNumber.Shapes[3]=textBoxPatternNumberMuzS4.Text;
                    CurrentPatternToNumber.Shapes[4]=textBoxPatternNumberMuzS5.Text;
                    CurrentPatternToNumber.Shapes[5]=textBoxPatternNumberMuzS6.Text;
                    CurrentPatternToNumber.Shapes[6]=textBoxPatternNumberMuzS7.Text;
                }
                if (CurrentPatternToNumber.ShowType==NumberType.Deklination || CurrentPatternToNumber.ShowType==NumberType.DeklinationWithGender) {
                    CurrentPatternToNumber.Shapes[7]=textBoxPatternNumberMuzM1.Text;
                    CurrentPatternToNumber.Shapes[8]=textBoxPatternNumberMuzM2.Text;
                    CurrentPatternToNumber.Shapes[9]=textBoxPatternNumberMuzM3.Text;
                    CurrentPatternToNumber.Shapes[10]=textBoxPatternNumberMuzM4.Text;
                    CurrentPatternToNumber.Shapes[11]=textBoxPatternNumberMuzM5.Text;
                    CurrentPatternToNumber.Shapes[12]=textBoxPatternNumberMuzM6.Text;
                    CurrentPatternToNumber.Shapes[13]=textBoxPatternNumberMuzM7.Text;
                }
                if (CurrentPatternToNumber.ShowType==NumberType.DeklinationWithGender) {
                    CurrentPatternToNumber.Shapes[14+0] =textBoxPatternNumberMunS1.Text;
                    CurrentPatternToNumber.Shapes[14+1] =textBoxPatternNumberMunS2.Text;
                    CurrentPatternToNumber.Shapes[14+2] =textBoxPatternNumberMunS3.Text;
                    CurrentPatternToNumber.Shapes[14+3] =textBoxPatternNumberMunS4.Text;
                    CurrentPatternToNumber.Shapes[14+4] =textBoxPatternNumberMunS5.Text;
                    CurrentPatternToNumber.Shapes[14+5] =textBoxPatternNumberMunS6.Text;
                    CurrentPatternToNumber.Shapes[14+6] =textBoxPatternNumberMunS7.Text;
                    CurrentPatternToNumber.Shapes[14+7] =textBoxPatternNumberMunM1.Text;
                    CurrentPatternToNumber.Shapes[14+8] =textBoxPatternNumberMunM2.Text;
                    CurrentPatternToNumber.Shapes[14+9] =textBoxPatternNumberMunM3.Text;
                    CurrentPatternToNumber.Shapes[14+10]=textBoxPatternNumberMunM4.Text;
                    CurrentPatternToNumber.Shapes[14+11]=textBoxPatternNumberMunM5.Text;
                    CurrentPatternToNumber.Shapes[14+12]=textBoxPatternNumberMunM6.Text;
                    CurrentPatternToNumber.Shapes[14+13]=textBoxPatternNumberMunM7.Text;

                    CurrentPatternToNumber.Shapes[28+0] =textBoxPatternNumberZenS1.Text;
                    CurrentPatternToNumber.Shapes[28+1] =textBoxPatternNumberZenS2.Text;
                    CurrentPatternToNumber.Shapes[28+2] =textBoxPatternNumberZenS3.Text;
                    CurrentPatternToNumber.Shapes[28+3] =textBoxPatternNumberZenS4.Text;
                    CurrentPatternToNumber.Shapes[28+4] =textBoxPatternNumberZenS5.Text;
                    CurrentPatternToNumber.Shapes[28+5] =textBoxPatternNumberZenS6.Text;
                    CurrentPatternToNumber.Shapes[28+6] =textBoxPatternNumberZenS7.Text;
                    CurrentPatternToNumber.Shapes[28+7] =textBoxPatternNumberZenM1.Text;
                    CurrentPatternToNumber.Shapes[28+8] =textBoxPatternNumberZenM2.Text;
                    CurrentPatternToNumber.Shapes[28+9] =textBoxPatternNumberZenM3.Text;
                    CurrentPatternToNumber.Shapes[28+10]=textBoxPatternNumberZenM4.Text;
                    CurrentPatternToNumber.Shapes[28+11]=textBoxPatternNumberZenM5.Text;
                    CurrentPatternToNumber.Shapes[28+12]=textBoxPatternNumberZenM6.Text;
                    CurrentPatternToNumber.Shapes[28+13]=textBoxPatternNumberZenM7.Text;

                    CurrentPatternToNumber.Shapes[42+0] =textBoxPatternNumberStrS1.Text;
                    CurrentPatternToNumber.Shapes[42+1] =textBoxPatternNumberStrS2.Text;
                    CurrentPatternToNumber.Shapes[42+2] =textBoxPatternNumberStrS3.Text;
                    CurrentPatternToNumber.Shapes[42+3] =textBoxPatternNumberStrS4.Text;
                    CurrentPatternToNumber.Shapes[42+4] =textBoxPatternNumberStrS5.Text;
                    CurrentPatternToNumber.Shapes[42+5] =textBoxPatternNumberStrS6.Text;
                    CurrentPatternToNumber.Shapes[42+6] =textBoxPatternNumberStrS7.Text;
                    CurrentPatternToNumber.Shapes[42+7] =textBoxPatternNumberStrM1.Text;
                    CurrentPatternToNumber.Shapes[42+8] =textBoxPatternNumberStrM2.Text;
                    CurrentPatternToNumber.Shapes[42+9] =textBoxPatternNumberStrM3.Text;
                    CurrentPatternToNumber.Shapes[42+10]=textBoxPatternNumberStrM4.Text;
                    CurrentPatternToNumber.Shapes[42+11]=textBoxPatternNumberStrM5.Text;
                    CurrentPatternToNumber.Shapes[42+12]=textBoxPatternNumberStrM6.Text;
                    CurrentPatternToNumber.Shapes[42+13]=textBoxPatternNumberStrM7.Text;
                }
            }
        } 
              
        void PatternNumberToSetNone(){ 
            textBoxPatternNumberName.Text="";

            textBoxPatternNumberFromMuzS1.Text="";
            textBoxPatternNumberFromMuzS2.Text="";
            textBoxPatternNumberFromMuzS3.Text="";
            textBoxPatternNumberMuzS4.Text="";
            textBoxPatternNumberMuzS5.Text="";
            textBoxPatternNumberMuzS6.Text="";
            textBoxPatternNumberMuzS7.Text="";
            textBoxPatternNumberMuzM1.Text="";
            textBoxPatternNumberMuzM2.Text="";
            textBoxPatternNumberMuzM3.Text="";
            textBoxPatternNumberMuzM4.Text="";
            textBoxPatternNumberMuzM5.Text="";
            textBoxPatternNumberMuzM6.Text="";
            textBoxPatternNumberMuzM7.Text="";

            textBoxPatternNumberMunS1.Text="";
            textBoxPatternNumberMunS2.Text="";
            textBoxPatternNumberMunS3.Text="";
            textBoxPatternNumberMunS4.Text="";
            textBoxPatternNumberMunS5.Text="";
            textBoxPatternNumberMunS6.Text="";
            textBoxPatternNumberMunS7.Text="";
            textBoxPatternNumberMunM1.Text="";
            textBoxPatternNumberMunM2.Text="";
            textBoxPatternNumberMunM3.Text="";
            textBoxPatternNumberMunM4.Text="";
            textBoxPatternNumberMunM5.Text="";
            textBoxPatternNumberMunM6.Text="";
            textBoxPatternNumberMunM7.Text="";

            textBoxPatternNumberZenS1.Text="";
            textBoxPatternNumberZenS2.Text="";
            textBoxPatternNumberZenS3.Text="";
            textBoxPatternNumberZenS4.Text="";
            textBoxPatternNumberZenS5.Text="";
            textBoxPatternNumberZenS6.Text="";
            textBoxPatternNumberZenS7.Text="";
            textBoxPatternNumberZenM1.Text="";
            textBoxPatternNumberZenM2.Text="";
            textBoxPatternNumberZenM3.Text="";
            textBoxPatternNumberZenM4.Text="";
            textBoxPatternNumberZenM5.Text="";
            textBoxPatternNumberZenM6.Text="";
            textBoxPatternNumberZenM7.Text="";

            textBoxPatternNumberStrS1.Text="";
            textBoxPatternNumberStrS2.Text="";
            textBoxPatternNumberStrS3.Text="";
            textBoxPatternNumberStrS4.Text="";
            textBoxPatternNumberStrS5.Text="";
            textBoxPatternNumberStrS6.Text="";
            textBoxPatternNumberStrS7.Text="";
            textBoxPatternNumberStrM1.Text="";
            textBoxPatternNumberStrM2.Text="";
            textBoxPatternNumberStrM3.Text="";
            textBoxPatternNumberStrM4.Text="";
            textBoxPatternNumberStrM5.Text="";
            textBoxPatternNumberStrM6.Text="";
            textBoxPatternNumberStrM7.Text="";

            labelPatternNumberName.Visible=false;
            tableLayoutPanelPatternNumberMuz.Visible=false;
            labelPatternNumberMuzFall.Visible=false;
            labelPatternNumberMuzMultiple.Visible=false;
            labelPatternNumberMuzSingle.Visible=false;
            labelPatternNumberType.Visible=false;
            comboBoxPatternNumberType.Visible=false;

            textBoxPatternNumberFromMuzS1.Visible=false;
            textBoxPatternNumberFromMuzS2.Visible=false;
            textBoxPatternNumberFromMuzS3.Visible=false;
            textBoxPatternNumberMuzS4.Visible=false;
            textBoxPatternNumberMuzS5.Visible=false;
            textBoxPatternNumberMuzS6.Visible=false;
            textBoxPatternNumberMuzS7.Visible=false;
            textBoxPatternNumberMuzM1.Visible=false;
            textBoxPatternNumberMuzM2.Visible=false;
            textBoxPatternNumberMuzM3.Visible=false;
            textBoxPatternNumberMuzM4.Visible=false;
            textBoxPatternNumberMuzM5.Visible=false;
            textBoxPatternNumberMuzM6.Visible=false;
            textBoxPatternNumberMuzM7.Visible=false;

            
            textBoxPatternNumberMunS1.Visible=false;
            textBoxPatternNumberMunS2.Visible=false;
            textBoxPatternNumberMunS3.Visible=false;
            textBoxPatternNumberMunS4.Visible=false;
            textBoxPatternNumberMunS5.Visible=false;
            textBoxPatternNumberMunS6.Visible=false;
            textBoxPatternNumberMunS7.Visible=false;
            textBoxPatternNumberMunM1.Visible=false;
            textBoxPatternNumberMunM2.Visible=false;
            textBoxPatternNumberMunM3.Visible=false;
            textBoxPatternNumberMunM4.Visible=false;
            textBoxPatternNumberMunM5.Visible=false;
            textBoxPatternNumberMunM6.Visible=false;
            textBoxPatternNumberMunM7.Visible=false;

            
            textBoxPatternNumberZenS1.Visible=false;
            textBoxPatternNumberZenS2.Visible=false;
            textBoxPatternNumberZenS3.Visible=false;
            textBoxPatternNumberZenS4.Visible=false;
            textBoxPatternNumberZenS5.Visible=false;
            textBoxPatternNumberZenS6.Visible=false;
            textBoxPatternNumberZenS7.Visible=false;
            textBoxPatternNumberZenM1.Visible=false;
            textBoxPatternNumberZenM2.Visible=false;
            textBoxPatternNumberZenM3.Visible=false;
            textBoxPatternNumberZenM4.Visible=false;
            textBoxPatternNumberZenM5.Visible=false;
            textBoxPatternNumberZenM6.Visible=false;
            textBoxPatternNumberZenM7.Visible=false;

            
            textBoxPatternNumberStrS1.Visible=false;
            textBoxPatternNumberStrS2.Visible=false;
            textBoxPatternNumberStrS3.Visible=false;
            textBoxPatternNumberStrS4.Visible=false;
            textBoxPatternNumberStrS5.Visible=false;
            textBoxPatternNumberStrS6.Visible=false;
            textBoxPatternNumberStrS7.Visible=false;
            textBoxPatternNumberStrM1.Visible=false;
            textBoxPatternNumberStrM2.Visible=false;
            textBoxPatternNumberStrM3.Visible=false;
            textBoxPatternNumberStrM4.Visible=false;
            textBoxPatternNumberStrM5.Visible=false;
            textBoxPatternNumberStrM6.Visible=false;
            textBoxPatternNumberStrM7.Visible=false;

            textBoxPatternNumberName.Visible=false;
        }
        
        void PatternNumberToClear() { 
            PatternNumberTolistBox.Items.Clear();
            PatternNumberToSetNone();
            itemsPatternNumbersToFiltered?.Clear();
            itemsPatternNumbersTo?.Clear();
            CurrentPatternToNumber=null;
        }
        #endregion

        #region Number
        void ListBoxNumber_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentNumber();
            
            int index=listBoxNumbers.SelectedIndex;
            if (itemsNumbers.Count==0) {
                NumberSetNone();
                return;
            }
            if (index>=itemsNumbers.Count) 
                index=itemsNumbers.Count-1;
            if (index<0)
                index=0;
           
            CurrentNumber=itemsNumbers[index];
            SetCurrentNumber();
            SetListBoxNumber();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void ButtonNumberAdd_Click(object sender, EventArgs e) {
            AddNewItemNumber();
        }

        void ButtonNumberRemove_Click(object sender, EventArgs e) {
            RemoveItemNumber(CurrentNumber);
            TextBoxNumberFilter_TextChanged(null, new EventArgs());
        }
        
        void TextBoxNumberFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentNumber();

            // Získej aktuální prvek
            ItemNumber selectedId=null;
            if (listBoxNumbers.SelectedIndex!=-1) {
                selectedId=itemsNumbersFiltered[listBoxNumbers.SelectedIndex];
            }
            
            NumberRefreshFilteredList();

            listBoxNumbers.Items.Clear();
            for (int i=0; i<itemsNumbersFiltered.Count; i++) {
                ItemNumber item = itemsNumbersFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxNumbers.Items.Add(textToAdd);
            }

            //SetListBoxNumber();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsNumbersFiltered.Count; i++){
                    if (itemsNumbersFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxNumbers.SelectedIndex=-1;
                    CurrentNumber=null;
                } else listBoxNumbers.SelectedIndex=outIndex;
            } else {
                listBoxNumbers.SelectedIndex=-1;
                CurrentNumber=null;
            }
            SetCurrentNumber();
        }
            
        void RemoveCurrentNumber(object sender, EventArgs e) {
            itemsNumbers.Remove(CurrentNumber);
        }

        void SetListBoxNumber() { 
            string filter=textBoxNumberFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxNumbers.SelectedIndex;
            listBoxNumbers.Items.Clear();
            for (int i=0; i<itemsNumbersFiltered.Count; i++) {
                ItemNumber item = itemsNumbersFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxNumbers.Items.Add(textToAdd);
            }

            if (index>=listBoxNumbers.Items.Count)index=listBoxNumbers.Items.Count-1;
            listBoxNumbers.SelectedIndex=index;
        }

        void NumberRefreshFilteredList() {
            if (itemsNumbersFiltered==null) itemsNumbersFiltered=new List<ItemNumber>();
            itemsNumbersFiltered.Clear();
            string filter=textBoxNumberFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsNumbers.Count; i++) {
                    ItemNumber item = itemsNumbers[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsNumbersFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsNumbers.Count; i++) {
                    ItemNumber item = itemsNumbers[i];
                    itemsNumbersFiltered.Add(item);
                }
            }
        }
               
        void AddNewItemNumber() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentNumber();

            var newItem=new ItemNumber();
           // newItem.ID=itemsNumbers.Count;
            itemsNumbers.Add(newItem);
            CurrentNumber=newItem;
            NumberRefreshFilteredList();
            SetListBoxNumber(); 
            ListBoxSetCurrentNumber();
            SetCurrentNumber(); 
            
            doingJob=false;
        }
   
        void RemoveItemNumber(ItemNumber item) { 
            Edited=true;
            ChangeCaptionText();
            itemsNumbers.Remove(item);
            NumberRefreshFilteredList();
            SetListBoxNumber();
            SetCurrentNumber();
        } 
           
        void SetCurrentNumber(){
            if (itemsNumbersFiltered.Count==0) {
                NumberSetNone();
                return;
            }

            int index=listBoxNumbers.SelectedIndex;
            if (index>=itemsNumbersFiltered.Count) index=itemsNumbersFiltered.Count-1;
            if (index<0) index=0;
            CurrentNumber=itemsNumbersFiltered[index];
            
            textBoxNumberFrom.Visible=true;
            textBoxNumberTo.Visible=true;
            labelNumberFrom.Visible=true;
            labelNumberTo.Visible=true; 

            textBoxNumberFrom.Text=CurrentNumber.From;
            textBoxNumberTo.Text=CurrentNumber.To;

            comboBoxNumberInputPatternFrom.Text=CurrentNumber.PatternFrom;

            comboBoxNumberInputPatternFrom.Items.Clear();
            comboBoxNumberInputPatternTo.Items.Clear();
            foreach (ItemPatternNumber x in itemsPatternNumbersFrom) {
                comboBoxNumberInputPatternFrom.Items.Add(x.Name);
                comboBoxNumberInputPatternTo.Items.Add(x.Name);
            }
            
            comboBoxNumberInputPatternTo.Text=CurrentNumber.PatternTo;
      
            comboBoxNumberInputPatternFrom.Visible=true; 
            comboBoxNumberInputPatternTo.Visible=true; 

            labelNumberInputPatternFrom.Visible=true; 
            labelNumberInputPatternTo.Visible=true; 

            labelNumberShowFrom.Visible=true;
            labelNumberShowTo.Visible=true;
        }
         
        void ListBoxSetCurrentNumber() {
            for (int indexCur=0; indexCur<itemsNumbersFiltered.Count; indexCur++) {
                if (itemsNumbers[indexCur]==CurrentNumber) { 
                    int indexList=listBoxNumbers.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxNumbers.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void SaveCurrentNumber() {
            if (CurrentNumber==null) return;
                     
            CurrentNumber.From=textBoxNumberFrom.Text;
            CurrentNumber.To=textBoxNumberTo.Text;
            
            CurrentNumber.PatternFrom=comboBoxNumberInputPatternFrom.Text;
            CurrentNumber.PatternTo=comboBoxNumberInputPatternTo.Text;
        } 
              
        void NumberSetNone(){ 
            textBoxNumberFrom.Text="";
            textBoxNumberTo.Text="";

            comboBoxNumberInputPatternFrom.Text="";
            comboBoxNumberInputPatternTo.Text="";

            textBoxNumberFrom.Visible=false;
            textBoxNumberTo.Visible=false;
            labelNumberFrom.Visible=false;
            labelNumberTo.Visible=false;
            comboBoxNumberInputPatternTo.Visible=false;
            comboBoxNumberInputPatternFrom.Visible=false;
            labelNumberShowFrom.Visible=false;
            labelNumberShowTo.Visible=false;
            labelNumberInputPatternFrom.Visible=false; 
            labelNumberInputPatternTo.Visible=false; 
        }
        
        void ClearNumber(){ 
            listBoxNumbers.Items.Clear();
            NumberSetNone();
            itemsNumbersFiltered?.Clear();
            itemsNumbers?.Clear();
            CurrentNumber=null;
        }
        #endregion
                
        #region Verb
        void ListBoxVerb_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentVerb();
            
            int index=listBoxVerbs.SelectedIndex;
            if (itemsVerbs.Count==0) {
                SetNoneVerb();
                return;
            }
            if (index>=itemsVerbs.Count) 
                index=itemsVerbs.Count-1;
            if (index<0)
                index=0;
           
            CurrentVerb=itemsVerbs[index];
            SetCurrentVerb();
            SetListBoxVerb();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void ButtonVerbAdd_Click(object sender, EventArgs e) {
            AddNewItemVerb();
        }

        void ButtonVerbRemove_Click(object sender, EventArgs e) {
            RemoveItemVerb(CurrentVerb);
            TextBoxVerbFilter_TextChanged(null, new EventArgs());
        }
        
        void TextBoxVerbFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentVerb();

            // Získej aktuální prvek
            ItemVerb selectedId=null;
            if (listBoxVerbs.SelectedIndex!=-1) {
                selectedId=itemsVerbsFiltered[listBoxVerbs.SelectedIndex];
            }
            
            VerbRefreshFilteredList();

            listBoxVerbs.Items.Clear();
            for (int i=0; i<itemsVerbsFiltered.Count; i++) {
                ItemVerb item = itemsVerbsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxVerbs.Items.Add(textToAdd);
            }

            //SetListBoxVerb();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsVerbsFiltered.Count; i++){
                    if (itemsVerbsFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxVerbs.SelectedIndex=-1;
                    CurrentVerb=null;
                } else listBoxVerbs.SelectedIndex=outIndex;
            } else {
                listBoxVerbs.SelectedIndex=-1;
                CurrentVerb=null;
            }
            SetCurrentVerb();
        }
            
        void RemoveCurrentVerb(object sender, EventArgs e) {
            itemsVerbs.Remove(CurrentVerb);
        }

        void SetListBoxVerb() { 
            string filter=textBoxVerbsFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxVerbs.SelectedIndex;
            listBoxVerbs.Items.Clear();
            for (int i=0; i<itemsVerbsFiltered.Count; i++) {
                ItemVerb item = itemsVerbsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxVerbs.Items.Add(textToAdd);
            }

            if (index>=listBoxVerbs.Items.Count)index=listBoxVerbs.Items.Count-1;
            listBoxVerbs.SelectedIndex=index;
        }

        void VerbRefreshFilteredList() {
            if (itemsVerbsFiltered==null) itemsVerbsFiltered=new List<ItemVerb>();
            itemsVerbsFiltered.Clear();
            string filter=textBoxVerbsFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsVerbs.Count; i++) {
                    ItemVerb item = itemsVerbs[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsVerbsFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsVerbs.Count; i++) {
                    ItemVerb item = itemsVerbs[i];
                    itemsVerbsFiltered.Add(item);
                }
            }
        }
               
        void AddNewItemVerb() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentVerb();

            var newItem=new ItemVerb();
           // newItem.ID=itemsVerbs.Count;
            itemsVerbs.Add(newItem);
            CurrentVerb=newItem;
            VerbRefreshFilteredList();
            SetListBoxVerb(); 
            ListBoxSetCurrentVerb();
            SetCurrentVerb(); 
            
            doingJob=false;
        }
   
        void RemoveItemVerb(ItemVerb item) { 
            Edited=true;
            ChangeCaptionText();
            itemsVerbs.Remove(item);
            VerbRefreshFilteredList();
            SetListBoxVerb();
            SetCurrentVerb();
        } 
           
        void SetCurrentVerb(){
            if (itemsVerbsFiltered.Count==0) {
                SetNoneVerb();
                return;
            }

            int index=listBoxVerbs.SelectedIndex;
            if (index>=itemsVerbsFiltered.Count) index=itemsVerbsFiltered.Count-1;
            if (index<0) index=0;
            CurrentVerb=itemsVerbsFiltered[index];
            
            textBoxVerbFrom.Visible=true;
            textBoxVerbTo.Visible=true;
            labelVerbFrom.Visible=true;
            labelVerbTo.Visible=true; 

            textBoxVerbFrom.Text=CurrentVerb.From;
            textBoxVerbTo.Text=CurrentVerb.To;

            comboBoxVerbInputPatternFrom.Text=CurrentVerb.PatternFrom;

            comboBoxVerbInputPatternFrom.Items.Clear();
            comboBoxVerbInputPatternTo.Items.Clear();
            foreach (ItemPatternVerb x in itemsPatternVerbFrom) {
                comboBoxVerbInputPatternFrom.Items.Add(x.Name);
                comboBoxVerbInputPatternTo.Items.Add(x.Name);
            }
            
            comboBoxVerbInputPatternTo.Text=CurrentVerb.PatternTo;
      
            comboBoxVerbInputPatternFrom.Visible=true; 
            comboBoxVerbInputPatternTo.Visible=true; 

            labelVerbInputPatternFrom.Visible=true; 
            labelVerbInputPatternTo.Visible=true; 

            labelVerbShowFrom.Visible=true;
            labelVerbShowTo.Visible=true;
        }
         
        void ListBoxSetCurrentVerb() {
            for (int indexCur=0; indexCur<itemsVerbsFiltered.Count; indexCur++) {
                if (itemsVerbs[indexCur]==CurrentVerb) { 
                    int indexList=listBoxVerbs.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxVerbs.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void SaveCurrentVerb() {
            if (CurrentVerb==null) return;
                     
            CurrentVerb.From=textBoxVerbFrom.Text;
            CurrentVerb.To=textBoxVerbTo.Text;
            
            CurrentVerb.PatternFrom=comboBoxVerbInputPatternFrom.Text;
            CurrentVerb.PatternTo=comboBoxVerbInputPatternTo.Text;
        } 
              
        void SetNoneVerb(){ 
            textBoxVerbFrom.Text="";
            textBoxVerbTo.Text="";

            comboBoxVerbInputPatternFrom.Text="";
            comboBoxVerbInputPatternTo.Text="";

            textBoxVerbFrom.Visible=false;
            textBoxVerbTo.Visible=false;
            labelVerbFrom.Visible=false;
            labelVerbTo.Visible=false;
            comboBoxVerbInputPatternTo.Visible=false;
            comboBoxVerbInputPatternFrom.Visible=false;
            labelVerbShowFrom.Visible=false;
            labelVerbShowTo.Visible=false;
            labelVerbInputPatternFrom.Visible=false; 
            labelVerbInputPatternTo.Visible=false; 
        }
        
        void ClearVerb() { 
            listBoxVerbs.Items.Clear();
            SetNoneVerb();
            itemsVerbsFiltered?.Clear();
            itemsVerbs?.Clear();
            CurrentVerb=null;
        }
        #endregion
 
        #region VerbPattern From 
        void PatternVerbFromcomboBoxShowType_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            if (CurrentPatternVerbFrom!=null) { 
                PatternVerbFromSaveCurrent();
                if (CurrentPatternVerbFrom.TypeShow!=(VerbTypeShow)comboBoxPatternVerbShowType.SelectedIndex) {
                    CurrentPatternVerbFrom.TypeShow=(VerbTypeShow)comboBoxPatternVerbShowType.SelectedIndex;
                    PatternVerbFromSetCurrent();
                }
            }
        }

        void PatternVerbFromListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternVerbFromSaveCurrent();
            
            int index=PatternVerbFromlistBox.SelectedIndex;
            if (itemsPatternVerbFrom.Count==0) {
                PatternVerbFromSetNone();
                return;
            }
            if (index>=itemsPatternVerbFrom.Count) index=itemsPatternVerbFrom.Count-1;
            if (index<0)  index=0;
           
            CurrentPatternVerbFrom=itemsPatternVerbFrom[index];
            PatternVerbFromSetCurrent();
            PatternVerbFromSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void PatternVerbFromButtonAdd_Click(object sender, EventArgs e) {
            PatternVerbFromAddNewItem();
        }

        void PatternVerbFromButtonRemove_Click(object sender, EventArgs e) {
            PatternVerbFromRemoveItem(CurrentPatternVerbFrom);
            PatternVerbFromTextBoxFilter_TextChanged(null, new EventArgs());
        }
        
        void PatternVerbFromTextBoxFilter_TextChanged(object sender, EventArgs e) {
            PatternVerbFromSaveCurrent();

            // Získej aktuální prvek
            ItemPatternVerb selectedId=null;
            if (PatternVerbFromlistBox.SelectedIndex!=-1) {
                selectedId=itemsPatternVerbsFromFiltered[PatternVerbFromlistBox.SelectedIndex];
            }
            
            PatternVerbFromRefreshFilteredList();

            PatternVerbFromlistBox.Items.Clear();
            for (int i=0; i<itemsPatternVerbsFromFiltered.Count; i++) {
                ItemPatternVerb item = itemsPatternVerbsFromFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                PatternVerbFromlistBox.Items.Add(textToAdd);
            }

            //SetListBoxVerb();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsPatternVerbsFromFiltered.Count; i++){
                    if (itemsPatternVerbsFromFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    PatternVerbFromlistBox.SelectedIndex=-1;
                    CurrentPatternVerbFrom=null;
                } else PatternVerbFromlistBox.SelectedIndex=outIndex;
            } else {
                PatternVerbFromlistBox.SelectedIndex=-1;
                CurrentPatternVerbFrom=null;
            }
            PatternVerbFromSetCurrent();
        }
            
        void PatternVerbFromRemoveCurrent(object sender, EventArgs e) {
            itemsPatternVerbFrom.Remove(CurrentPatternVerbFrom);
        }

        void PatternVerbFromSetListBox() { 
            string filter=textBoxPatternVerbsFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=PatternVerbFromlistBox.SelectedIndex;
            PatternVerbFromlistBox.Items.Clear();
            for (int i=0; i<itemsPatternVerbsFromFiltered.Count; i++) {
                ItemPatternVerb item = itemsPatternVerbsFromFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                PatternVerbFromlistBox.Items.Add(textToAdd);
            }

            if (index>=PatternVerbFromlistBox.Items.Count)index=PatternVerbFromlistBox.Items.Count-1;
            PatternVerbFromlistBox.SelectedIndex=index;
        }

        void PatternVerbFromRefreshFilteredList() {
            if (itemsPatternVerbsFromFiltered==null) itemsPatternVerbsFromFiltered=new List<ItemPatternVerb>();
            itemsPatternVerbsFromFiltered.Clear();
            string filter=textBoxPatternVerbsFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsPatternVerbFrom.Count; i++) {
                    ItemPatternVerb item = itemsPatternVerbFrom[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsPatternVerbsFromFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsPatternVerbFrom.Count; i++) {
                    ItemPatternVerb item = itemsPatternVerbFrom[i];
                    itemsPatternVerbsFromFiltered.Add(item);
                }
            }
        }
               
        void PatternVerbFromAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternVerbFromSaveCurrent();

            var newItem=new ItemPatternVerb();
           // newItem.ID=itemsVerbs.Count;
            itemsPatternVerbFrom.Add(newItem);
            CurrentPatternVerbFrom=newItem;
            PatternVerbFromRefreshFilteredList();
            PatternVerbFromSetListBox(); 
            PatternVerbFromListBoxSetCurrent();
            PatternVerbFromSetCurrent(); 
            
            doingJob=false;
        }
   
        void PatternVerbFromRemoveItem(ItemPatternVerb item) { 
            Edited=true;
            ChangeCaptionText();
            itemsPatternVerbFrom.Remove(item);
            PatternVerbFromRefreshFilteredList();
            PatternVerbFromSetListBox();
            PatternVerbFromSetCurrent();
        } 
           
        void PatternVerbFromSetCurrent() {
            if (itemsPatternVerbsFromFiltered.Count==0) {
                PatternVerbFromSetNone();
                return;
            }

            int index=PatternVerbFromlistBox.SelectedIndex;
            if (index>=itemsPatternVerbsFromFiltered.Count) index=itemsPatternVerbsFromFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternVerbFrom=itemsPatternVerbsFromFiltered[index];

            textBoxPatternVerbName.Text=CurrentPatternVerbFrom.Name;
            comboBoxPatternVerbType.SelectedIndex=(int)CurrentPatternVerbFrom.Type;
            if (comboBoxPatternVerbShowType.SelectedIndex!=(int)CurrentPatternVerbFrom.TypeShow)comboBoxPatternVerbShowType.SelectedIndex=(int)CurrentPatternVerbFrom.TypeShow;
            textBoxPatternVerbInfinitive.Text=CurrentPatternVerbFrom.Infinitive;
            textBoxPatternVerbInfinitive.Visible=true;

            if (CurrentPatternVerbFrom.TypeShow==VerbTypeShow.FutureActive || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.FuturePassive){ 
                tableLayoutPanelPatternVerbContinous.Visible=false;
                labelPatternVerbContinous.Visible=false;
                labelPatternVerbContinousMultiple.Visible=false;
                labelPatternVerbContinousSingle.Visible=false; 
                labelPatternVerbContinousPeople.Visible=false;
            } else {
                textBoxPatternVerbPrJ1.Text=CurrentPatternVerbFrom.Continous[0];
                textBoxPatternVerbPrJ2.Text=CurrentPatternVerbFrom.Continous[1];
                textBoxPatternVerbPrJ3.Text=CurrentPatternVerbFrom.Continous[2];
                textBoxPatternVerbPrM1.Text=CurrentPatternVerbFrom.Continous[3];
                textBoxPatternVerbPrM2.Text=CurrentPatternVerbFrom.Continous[4];
                textBoxPatternVerbPrM3.Text=CurrentPatternVerbFrom.Continous[5];     
            
                tableLayoutPanelPatternVerbContinous.Visible=true;
                labelPatternVerbContinous.Visible=true;
                labelPatternVerbContinousMultiple.Visible=true;
                labelPatternVerbContinousSingle.Visible=true; 
                labelPatternVerbContinousPeople.Visible=true;
            }

            if (CurrentPatternVerbFrom.TypeShow==VerbTypeShow.All || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.Unknown || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.FutureActive || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.FuturePassive) {
                textBoxPatternVerbBuJ1.Text=CurrentPatternVerbFrom.Future[0];
                textBoxPatternVerbBuJ2.Text=CurrentPatternVerbFrom.Future[1];
                textBoxPatternVerbBuJ3.Text=CurrentPatternVerbFrom.Future[2];
                textBoxPatternVerbBuM1.Text=CurrentPatternVerbFrom.Future[3];
                textBoxPatternVerbBuM2.Text=CurrentPatternVerbFrom.Future[4];
                textBoxPatternVerbBuM3.Text=CurrentPatternVerbFrom.Future[5];
                
                tableLayoutPanelPatternVerbFuture.Visible=true;
                labelPatternVerbFuture.Visible=true;
                labelPatternVerbFutureMultiple.Visible=true;
                labelPatternVerbFuturePeople.Visible=true;
                labelPatternVerbFutureSingle.Visible=true;
            } else { 
                tableLayoutPanelPatternVerbFuture.Visible=false;
                labelPatternVerbFuture.Visible=false;
                labelPatternVerbFutureMultiple.Visible=false;
                labelPatternVerbFuturePeople.Visible=false;
                labelPatternVerbFutureSingle.Visible=false;
            }

            if (CurrentPatternVerbFrom.TypeShow==VerbTypeShow.All || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.Unknown){
                textBoxPatternVerbAux1.Text=CurrentPatternVerbFrom.Auxiliary[0];
                textBoxPatternVerbAux2.Text=CurrentPatternVerbFrom.Auxiliary[1];
                textBoxPatternVerbAux3.Text=CurrentPatternVerbFrom.Auxiliary[2];
                textBoxPatternVerbAux4.Text=CurrentPatternVerbFrom.Auxiliary[3];
                textBoxPatternVerbAux5.Text=CurrentPatternVerbFrom.Auxiliary[4];
                textBoxPatternVerbAux6.Text=CurrentPatternVerbFrom.Auxiliary[5];

                textBoxPatternVerbAux1.Visible=true;
                textBoxPatternVerbAux2.Visible=true;
                textBoxPatternVerbAux3.Visible=true;
                textBoxPatternVerbAux4.Visible=true;
                textBoxPatternVerbAux5.Visible=true;
                textBoxPatternVerbAux6.Visible=true;
                tableLayoutPanelPatternVerbAux.Visible=true;
                labelPatternVerbAuxText.Visible=true;
                labelPatternVerbSingle.Visible=true;
                labelPatternVerbMultiple.Visible=true;
            }else{ 
              

                textBoxPatternVerbAux1.Visible=false;
                textBoxPatternVerbAux2.Visible=false;
                textBoxPatternVerbAux3.Visible=false;
                textBoxPatternVerbAux4.Visible=false;
                textBoxPatternVerbAux5.Visible=false;
                textBoxPatternVerbAux6.Visible=false;
                tableLayoutPanelPatternVerbAux.Visible=false;
                labelPatternVerbAuxText.Visible=false;
                labelPatternVerbSingle.Visible=false;
                labelPatternVerbMultiple.Visible=false;
            }

            textBoxPatternVerbRoJ2.Text=CurrentPatternVerbFrom.Imperative[0];
            textBoxPatternVerbRoM1.Text=CurrentPatternVerbFrom.Imperative[1];
            textBoxPatternVerbRoM2.Text=CurrentPatternVerbFrom.Imperative[2];
            tableLayoutPanelPatternVerbImperative.Visible=true;
            labelPatternVerbImperative.Visible=true;
            labelPatternVerbImperativeMultiple.Visible=true;
            labelPatternVerbImperativePeople.Visible=true;
            labelPatternVerbImperativeSingle.Visible=true;
                        
            if (CurrentPatternVerbFrom.TypeShow==VerbTypeShow.Trpne || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.TrpneCinne || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.All || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.Unknown) {
                textBoxPatternVerbMtMzJ.Text=CurrentPatternVerbFrom.PastPassive[0];
                textBoxPatternVerbMtMnJ.Text=CurrentPatternVerbFrom.PastPassive[1];
                textBoxPatternVerbMtZeJ.Text=CurrentPatternVerbFrom.PastPassive[2];
                textBoxPatternVerbMtStJ.Text=CurrentPatternVerbFrom.PastPassive[3];
                textBoxPatternVerbMtMzM.Text=CurrentPatternVerbFrom.PastPassive[4];
                textBoxPatternVerbMtMnM.Text=CurrentPatternVerbFrom.PastPassive[5];
                textBoxPatternVerbMtZeM.Text=CurrentPatternVerbFrom.PastPassive[6];
                textBoxPatternVerbMtStM.Text=CurrentPatternVerbFrom.PastPassive[7];  
                
                tableLayoutPanelPatternVerbPastInactive.Visible=true;
                labelPatternVerbPastInactive.Visible=true;
                labelPatternVerbPastInactiveMultiple.Visible=true;
                labelPatternVerbPastInactivePeople.Visible=true;
                labelPatternVerbPastInactiveSingle.Visible=true;
            } else { 
                tableLayoutPanelPatternVerbPastInactive.Visible=false;
                labelPatternVerbPastInactive.Visible=false;
                labelPatternVerbPastInactiveMultiple.Visible=false;
                labelPatternVerbPastInactivePeople.Visible=false;
                labelPatternVerbPastInactiveSingle.Visible=false;
            }

            if (CurrentPatternVerbFrom.TypeShow==VerbTypeShow.Cinne || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.TrpneCinne || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.All || CurrentPatternVerbFrom.TypeShow==VerbTypeShow.Unknown) {
                textBoxPatternVerbMcMzJ.Text=CurrentPatternVerbFrom.PastActive[0];
                textBoxPatternVerbMcMnJ.Text=CurrentPatternVerbFrom.PastActive[1];
                textBoxPatternVerbMcZeJ.Text=CurrentPatternVerbFrom.PastActive[2];
                textBoxPatternVerbMcStJ.Text=CurrentPatternVerbFrom.PastActive[3];
                textBoxPatternVerbMcMzM.Text=CurrentPatternVerbFrom.PastActive[4];
                textBoxPatternVerbMcMnM.Text=CurrentPatternVerbFrom.PastActive[5];
                textBoxPatternVerbMcZeM.Text=CurrentPatternVerbFrom.PastActive[6];
                textBoxPatternVerbMcStM.Text=CurrentPatternVerbFrom.PastActive[7];

                tableLayoutPanelPatternVerbPastActive.Visible=true;
                labelVerbPatternPastActive.Visible=true;
                labelVerbPatternPastActiveMultiple.Visible=true;
                labelVerbPatternPastActivePeople.Visible=true;
                labelVerbPatternPastActiveSingle.Visible=true;
            }else{ 
                 tableLayoutPanelPatternVerbPastActive.Visible=false;
                labelVerbPatternPastActive.Visible=false;
                labelVerbPatternPastActiveMultiple.Visible=false;
                labelVerbPatternPastActivePeople.Visible=false;
                labelVerbPatternPastActiveSingle.Visible=false;
            }

            textBoxPatternVerbTr1.Text=CurrentPatternVerbFrom.TransgressiveCont[0];
            textBoxPatternVerbTr2.Text=CurrentPatternVerbFrom.TransgressiveCont[1];
            textBoxPatternVerbTr3.Text=CurrentPatternVerbFrom.TransgressiveCont[2];
            textBoxPatternVerbTr4.Text=CurrentPatternVerbFrom.TransgressivePast[0];
            textBoxPatternVerbTr5.Text=CurrentPatternVerbFrom.TransgressivePast[1];
            textBoxPatternVerbTr6.Text=CurrentPatternVerbFrom.TransgressivePast[2];

            tableLayoutPanelPatternVerbTransgressive.Visible=true;
            labelPatternVerbTransgressive.Visible=true; 
            PatternVerbTransgressiveContinous.Visible=true;
            PatternVerbTransgressivePast.Visible=true;
          
            textBoxPatternVerbTr1.Text=CurrentPatternVerbFrom.TransgressiveCont[0];
            textBoxPatternVerbTr2.Text=CurrentPatternVerbFrom.TransgressiveCont[1];
            textBoxPatternVerbTr3.Text=CurrentPatternVerbFrom.TransgressiveCont[2];
            textBoxPatternVerbTr4.Text=CurrentPatternVerbFrom.TransgressivePast[0];
            textBoxPatternVerbTr5.Text=CurrentPatternVerbFrom.TransgressivePast[1];
            textBoxPatternVerbTr6.Text=CurrentPatternVerbFrom.TransgressivePast[2];


            textBoxPatternVerbName.Visible=true;
            labelVerbPatterName.Visible=true;
            labelPatternVerbType.Visible=true;
          
            comboBoxPatternVerbType.Visible=true;
          
            
          
           
          
            textBoxPatternVerbInfinitive.Visible=true;
            labelPatternVerbInfinitive.Visible=true;
        }
         
        void PatternVerbFromListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternVerbsFromFiltered.Count; indexCur++) {
                if (itemsPatternVerbFrom[indexCur]==CurrentPatternVerbFrom) { 
                    int indexList=PatternVerbFromlistBox.SelectedIndex;
                    if (indexList==indexCur) return;
                    PatternVerbFromlistBox.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void PatternVerbFromSaveCurrent() {
            if (CurrentPatternVerbFrom==null) return;
                     
            CurrentPatternVerbFrom.Name=textBoxPatternVerbName.Text;
            CurrentPatternVerbFrom.Type=(VerbType)comboBoxPatternVerbType.SelectedIndex;
            CurrentPatternVerbFrom.TypeShow=(VerbTypeShow)comboBoxPatternVerbShowType.SelectedIndex;
            CurrentPatternVerbFrom.Infinitive=textBoxPatternVerbInfinitive.Text;

            CurrentPatternVerbFrom.Continous[0]=textBoxPatternVerbPrJ1.Text;
            CurrentPatternVerbFrom.Continous[1]=textBoxPatternVerbPrJ2.Text;
            CurrentPatternVerbFrom.Continous[2]=textBoxPatternVerbPrJ3.Text;
            CurrentPatternVerbFrom.Continous[3]=textBoxPatternVerbPrM1.Text;
            CurrentPatternVerbFrom.Continous[4]=textBoxPatternVerbPrM2.Text;
            CurrentPatternVerbFrom.Continous[5]=textBoxPatternVerbPrM3.Text;

            CurrentPatternVerbFrom.Future[0]=textBoxPatternVerbBuJ1.Text;
            CurrentPatternVerbFrom.Future[1]=textBoxPatternVerbBuJ2.Text;
            CurrentPatternVerbFrom.Future[2]=textBoxPatternVerbBuJ3.Text;
            CurrentPatternVerbFrom.Future[3]=textBoxPatternVerbBuM1.Text;
            CurrentPatternVerbFrom.Future[4]=textBoxPatternVerbBuM2.Text;
            CurrentPatternVerbFrom.Future[5]=textBoxPatternVerbBuM3.Text;

            CurrentPatternVerbFrom.Imperative[0]=textBoxPatternVerbRoJ2.Text;
            CurrentPatternVerbFrom.Imperative[1]=textBoxPatternVerbRoM1.Text;
            CurrentPatternVerbFrom.Imperative[2]=textBoxPatternVerbRoM2.Text;
                        
            CurrentPatternVerbFrom.PastPassive[0]=textBoxPatternVerbMtMzJ.Text;
            CurrentPatternVerbFrom.PastPassive[1]=textBoxPatternVerbMtMnJ.Text;
            CurrentPatternVerbFrom.PastPassive[2]=textBoxPatternVerbMtZeJ.Text;
            CurrentPatternVerbFrom.PastPassive[3]=textBoxPatternVerbMtStJ.Text;
            CurrentPatternVerbFrom.PastPassive[4]=textBoxPatternVerbMtMzM.Text;
            CurrentPatternVerbFrom.PastPassive[5]=textBoxPatternVerbMtMnM.Text;
            CurrentPatternVerbFrom.PastPassive[6]=textBoxPatternVerbMtZeM.Text;
            CurrentPatternVerbFrom.PastPassive[7]=textBoxPatternVerbMtStM.Text;
            
            CurrentPatternVerbFrom.PastActive[0]=textBoxPatternVerbMcMzJ.Text;
            CurrentPatternVerbFrom.PastActive[1]=textBoxPatternVerbMcMnJ.Text;
            CurrentPatternVerbFrom.PastActive[2]=textBoxPatternVerbMcZeJ.Text;
            CurrentPatternVerbFrom.PastActive[3]=textBoxPatternVerbMcStJ.Text;
            CurrentPatternVerbFrom.PastActive[4]=textBoxPatternVerbMcMzM.Text;
            CurrentPatternVerbFrom.PastActive[5]=textBoxPatternVerbMcMnM.Text;
            CurrentPatternVerbFrom.PastActive[6]=textBoxPatternVerbMcZeM.Text;
            CurrentPatternVerbFrom.PastActive[7]=textBoxPatternVerbMcStM.Text;

            CurrentPatternVerbFrom.TransgressiveCont[0]=textBoxPatternVerbTr1.Text;
            CurrentPatternVerbFrom.TransgressiveCont[1]=textBoxPatternVerbTr2.Text;
            CurrentPatternVerbFrom.TransgressiveCont[2]=textBoxPatternVerbTr3.Text;
            CurrentPatternVerbFrom.TransgressivePast[0]=textBoxPatternVerbTr4.Text;
            CurrentPatternVerbFrom.TransgressivePast[1]=textBoxPatternVerbTr5.Text;
            CurrentPatternVerbFrom.TransgressivePast[2]=textBoxPatternVerbTr6.Text;

            CurrentPatternVerbFrom.Auxiliary[0]=textBoxPatternVerbAux1.Text;
            CurrentPatternVerbFrom.Auxiliary[1]=textBoxPatternVerbAux2.Text;
            CurrentPatternVerbFrom.Auxiliary[2]=textBoxPatternVerbAux3.Text;
            CurrentPatternVerbFrom.Auxiliary[3]=textBoxPatternVerbAux4.Text;
            CurrentPatternVerbFrom.Auxiliary[4]=textBoxPatternVerbAux5.Text;
            CurrentPatternVerbFrom.Auxiliary[5]=textBoxPatternVerbAux6.Text;
        } 
              
        void PatternVerbFromSetNone(){ 
            textBoxPatternVerbName.Text="";

            textBoxPatternVerbPrJ1.Text="";
            textBoxPatternVerbPrJ2.Text="";
            textBoxPatternVerbPrJ3.Text="";
            textBoxPatternVerbPrM1.Text="";
            textBoxPatternVerbPrM2.Text="";
            textBoxPatternVerbPrM3.Text="";

            textBoxPatternVerbBuJ1.Text="";
            textBoxPatternVerbBuJ2.Text="";
            textBoxPatternVerbBuJ3.Text="";
            textBoxPatternVerbBuM1.Text="";
            textBoxPatternVerbBuM2.Text="";
            textBoxPatternVerbBuM3.Text="";

            textBoxPatternVerbRoJ2.Text="";
            textBoxPatternVerbRoM1.Text="";
            textBoxPatternVerbRoM2.Text="";
                        
            textBoxPatternVerbMtMzJ.Text="";
            textBoxPatternVerbMtMnJ.Text="";
            textBoxPatternVerbMtZeJ.Text="";
            textBoxPatternVerbMtStJ.Text="";
            textBoxPatternVerbMtMzM.Text="";
            textBoxPatternVerbMtMnM.Text="";
            textBoxPatternVerbMtZeM.Text="";
            textBoxPatternVerbMtStM.Text="";
            
            textBoxPatternVerbMcMzJ.Text="";
            textBoxPatternVerbMcMnJ.Text="";
            textBoxPatternVerbMcZeJ.Text="";
            textBoxPatternVerbMcStJ.Text="";
            textBoxPatternVerbMcMzM.Text="";
            textBoxPatternVerbMcMnM.Text="";
            textBoxPatternVerbMcZeM.Text="";
            textBoxPatternVerbMcStM.Text="";

            
            textBoxPatternVerbTr1.Text="";
            textBoxPatternVerbTr2.Text="";
            textBoxPatternVerbTr3.Text="";
            textBoxPatternVerbTr4.Text="";
            textBoxPatternVerbTr5.Text="";
            textBoxPatternVerbTr6.Text="";

            labelVerbPatterName.Visible=false;
            labelPatternVerbType.Visible=false;
            tableLayoutPanelPatternVerbContinous.Visible=false;
            labelPatternVerbContinous.Visible=false;
            labelPatternVerbContinousMultiple.Visible=false;
            labelPatternVerbContinousSingle.Visible=false;
            PatternVerbTransgressiveContinous.Visible=false;
            tableLayoutPanelPatternVerbFuture.Visible=false;
            labelPatternVerbFuture.Visible=false;
            labelPatternVerbFutureMultiple.Visible=false;
            labelPatternVerbFuturePeople.Visible=false;
            labelPatternVerbFutureSingle.Visible=false;
            comboBoxPatternVerbType.Visible=false;
            PatternVerbTransgressivePast.Visible=false;
            tableLayoutPanelPatternVerbImperative.Visible=false;
            labelPatternVerbImperative.Visible=false;
            labelPatternVerbImperativeMultiple.Visible=false;
            labelPatternVerbImperativePeople.Visible=false;
            labelPatternVerbImperativeSingle.Visible=false;
            labelPatternVerbContinousPeople.Visible=false;
            tableLayoutPanelPatternVerbPastInactive.Visible=false;
            labelPatternVerbPastInactive.Visible=false;
            labelPatternVerbPastInactiveMultiple.Visible=false;
            labelPatternVerbPastInactivePeople.Visible=false;
            labelPatternVerbPastInactiveSingle.Visible=false;
            textBoxPatternVerbName.Visible=false;
            tableLayoutPanelPatternVerbPastActive.Visible=false;
            labelVerbPatternPastActive.Visible=false;
            labelVerbPatternPastActiveMultiple.Visible=false;
            labelVerbPatternPastActivePeople.Visible=false;
            labelVerbPatternPastActiveSingle.Visible=false;

            tableLayoutPanelPatternVerbTransgressive.Visible=false;
            labelPatternVerbTransgressive.Visible=false; 
        }
        
        void PatternVerbFromClear() { 
            PatternVerbFromlistBox.Items.Clear();
            PatternVerbFromSetNone();
            itemsPatternVerbsFromFiltered?.Clear();
            itemsPatternVerbFrom?.Clear();
            CurrentPatternVerbFrom=null;
        }
        #endregion
           
        #region VerbPattern To 
        void PatternVerbTocomboBoxShowType_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            if (CurrentPatternVerbTo!=null) { 
                PatternVerbToSaveCurrent();
                if (CurrentPatternVerbTo.TypeShow!=(VerbTypeShow)comboBoxPatternVerbShowType.SelectedIndex) {
                    CurrentPatternVerbTo.TypeShow=(VerbTypeShow)comboBoxPatternVerbShowType.SelectedIndex;
                    PatternVerbToSetCurrent();
                }
            }
        }

        void PatternVerbToListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            PatternVerbToSaveCurrent();
            
            int index=PatternVerbToListBox.SelectedIndex;
            if (itemsPatternVerbTo.Count==0) {
                PatternVerbToSetNone();
                return;
            }
            if (index>=itemsPatternVerbTo.Count) index=itemsPatternVerbTo.Count-1;
            if (index<0)  index=0;
           
            CurrentPatternVerbTo=itemsPatternVerbTo[index];
            PatternVerbToSetCurrent();
            PatternVerbToSetListBox();
          //  SetCurrent();
            doingJob=false;
        }

        void PatternVerbToButtonAdd_Click(object sender, EventArgs e) {
            PatternVerbToAddNewItem();
        }

        void PatternVerbToButtonRemove_Click(object sender, EventArgs e) {
            PatternVerbToRemoveItem(CurrentPatternVerbTo);
            PatternVerbToTextBoxFilter_TextChanged(null, new EventArgs());
        }
        
        void PatternVerbToTextBoxFilter_TextChanged(object sender, EventArgs e) {
            PatternVerbToSaveCurrent();

            // Získej aktuální prvek
            ItemPatternVerb selectedId=null;
            if (PatternVerbToListBox.SelectedIndex!=-1) {
                selectedId=itemsPatternVerbsToFiltered[PatternVerbToListBox.SelectedIndex];
            }
            
            PatternVerbToRefreshFilteredList();

            PatternVerbToListBox.Items.Clear();
            for (int i=0; i<itemsPatternVerbsToFiltered.Count; i++) {
                ItemPatternVerb item = itemsPatternVerbsToFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                PatternVerbToListBox.Items.Add(textToAdd);
            }

            //SetListBoxVerb();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsPatternVerbsToFiltered.Count; i++){
                    if (itemsPatternVerbsToFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    PatternVerbToListBox.SelectedIndex=-1;
                    CurrentPatternVerbTo=null;
                } else PatternVerbToListBox.SelectedIndex=outIndex;
            } else {
                PatternVerbToListBox.SelectedIndex=-1;
                CurrentPatternVerbTo=null;
            }
            PatternVerbToSetCurrent();
        }
            
        void PatternVerbToRemoveCurrent(object sender, EventArgs e) {
            itemsPatternVerbTo.Remove(CurrentPatternVerbTo);
        }

        void PatternVerbToSetListBox() { 
            string filter=textBoxPatternVerbsFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=PatternVerbToListBox.SelectedIndex;
            PatternVerbToListBox.Items.Clear();
            for (int i=0; i<itemsPatternVerbsToFiltered.Count; i++) {
                ItemPatternVerb item = itemsPatternVerbsToFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                PatternVerbToListBox.Items.Add(textToAdd);
            }

            if (index>=PatternVerbToListBox.Items.Count)index=PatternVerbToListBox.Items.Count-1;
            PatternVerbToListBox.SelectedIndex=index;
        }

        void PatternVerbToRefreshFilteredList() {
            if (itemsPatternVerbsToFiltered==null) itemsPatternVerbsToFiltered=new List<ItemPatternVerb>();
            itemsPatternVerbsToFiltered.Clear();
            string filter=textBoxPatternVerbsFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsPatternVerbTo.Count; i++) {
                    ItemPatternVerb item = itemsPatternVerbTo[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsPatternVerbsToFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsPatternVerbTo.Count; i++) {
                    ItemPatternVerb item = itemsPatternVerbTo[i];
                    itemsPatternVerbsToFiltered.Add(item);
                }
            }
        }
               
        void PatternVerbToAddNewItem() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            PatternVerbToSaveCurrent();

            var newItem=new ItemPatternVerb();
           // newItem.ID=itemsVerbs.Count;
            itemsPatternVerbTo.Add(newItem);
            CurrentPatternVerbTo=newItem;
            PatternVerbToRefreshFilteredList();
            PatternVerbToSetListBox(); 
            PatternVerbToListBoxSetCurrent();
            PatternVerbToSetCurrent(); 
            
            doingJob=false;
        }
   
        void PatternVerbToRemoveItem(ItemPatternVerb item) { 
            Edited=true;
            ChangeCaptionText();
            itemsPatternVerbTo.Remove(item);
            PatternVerbToRefreshFilteredList();
            PatternVerbToSetListBox();
            PatternVerbToSetCurrent();
        } 
           
        void PatternVerbToSetCurrent() {
            if (itemsPatternVerbsToFiltered.Count==0) {
                PatternVerbToSetNone();
                return;
            }

            int index=PatternVerbToListBox.SelectedIndex;
            if (index>=itemsPatternVerbsToFiltered.Count) index=itemsPatternVerbsToFiltered.Count-1;
            if (index<0) index=0;
            CurrentPatternVerbTo=itemsPatternVerbsToFiltered[index];

            textBoxPatternVerbName.Text=CurrentPatternVerbTo.Name;
            comboBoxPatternVerbType.SelectedIndex=(int)CurrentPatternVerbTo.Type;
            if (comboBoxPatternVerbShowType.SelectedIndex!=(int)CurrentPatternVerbTo.TypeShow)comboBoxPatternVerbShowType.SelectedIndex=(int)CurrentPatternVerbTo.TypeShow;
            textBoxPatternVerbInfinitive.Text=CurrentPatternVerbTo.Infinitive;
            textBoxPatternVerbInfinitive.Visible=true;

            if (CurrentPatternVerbTo.TypeShow==VerbTypeShow.FutureActive || CurrentPatternVerbTo.TypeShow==VerbTypeShow.FuturePassive){ 
                tableLayoutPanelPatternVerbContinous.Visible=false;
                labelPatternVerbContinous.Visible=false;
                labelPatternVerbContinousMultiple.Visible=false;
                labelPatternVerbContinousSingle.Visible=false; 
                labelPatternVerbContinousPeople.Visible=false;
            } else {
                textBoxPatternVerbPrJ1.Text=CurrentPatternVerbTo.Continous[0];
                textBoxPatternVerbPrJ2.Text=CurrentPatternVerbTo.Continous[1];
                textBoxPatternVerbPrJ3.Text=CurrentPatternVerbTo.Continous[2];
                textBoxPatternVerbPrM1.Text=CurrentPatternVerbTo.Continous[3];
                textBoxPatternVerbPrM2.Text=CurrentPatternVerbTo.Continous[4];
                textBoxPatternVerbPrM3.Text=CurrentPatternVerbTo.Continous[5];     
            
                tableLayoutPanelPatternVerbContinous.Visible=true;
                labelPatternVerbContinous.Visible=true;
                labelPatternVerbContinousMultiple.Visible=true;
                labelPatternVerbContinousSingle.Visible=true; 
                labelPatternVerbContinousPeople.Visible=true;
            }

            if (CurrentPatternVerbTo.TypeShow==VerbTypeShow.All || CurrentPatternVerbTo.TypeShow==VerbTypeShow.Unknown || CurrentPatternVerbTo.TypeShow==VerbTypeShow.FutureActive || CurrentPatternVerbTo.TypeShow==VerbTypeShow.FuturePassive) {
                textBoxPatternVerbBuJ1.Text=CurrentPatternVerbTo.Future[0];
                textBoxPatternVerbBuJ2.Text=CurrentPatternVerbTo.Future[1];
                textBoxPatternVerbBuJ3.Text=CurrentPatternVerbTo.Future[2];
                textBoxPatternVerbBuM1.Text=CurrentPatternVerbTo.Future[3];
                textBoxPatternVerbBuM2.Text=CurrentPatternVerbTo.Future[4];
                textBoxPatternVerbBuM3.Text=CurrentPatternVerbTo.Future[5];
                
                tableLayoutPanelPatternVerbFuture.Visible=true;
                labelPatternVerbFuture.Visible=true;
                labelPatternVerbFutureMultiple.Visible=true;
                labelPatternVerbFuturePeople.Visible=true;
                labelPatternVerbFutureSingle.Visible=true;
            } else { 
                tableLayoutPanelPatternVerbFuture.Visible=false;
                labelPatternVerbFuture.Visible=false;
                labelPatternVerbFutureMultiple.Visible=false;
                labelPatternVerbFuturePeople.Visible=false;
                labelPatternVerbFutureSingle.Visible=false;
            }

            if (CurrentPatternVerbTo.TypeShow==VerbTypeShow.All || CurrentPatternVerbTo.TypeShow==VerbTypeShow.Unknown){
                textBoxPatternVerbAux1.Text=CurrentPatternVerbTo.Auxiliary[0];
                textBoxPatternVerbAux2.Text=CurrentPatternVerbTo.Auxiliary[1];
                textBoxPatternVerbAux3.Text=CurrentPatternVerbTo.Auxiliary[2];
                textBoxPatternVerbAux4.Text=CurrentPatternVerbTo.Auxiliary[3];
                textBoxPatternVerbAux5.Text=CurrentPatternVerbTo.Auxiliary[4];
                textBoxPatternVerbAux6.Text=CurrentPatternVerbTo.Auxiliary[5];

                textBoxPatternVerbAux1.Visible=true;
                textBoxPatternVerbAux2.Visible=true;
                textBoxPatternVerbAux3.Visible=true;
                textBoxPatternVerbAux4.Visible=true;
                textBoxPatternVerbAux5.Visible=true;
                textBoxPatternVerbAux6.Visible=true;
                tableLayoutPanelPatternVerbAux.Visible=true;
                labelPatternVerbAuxText.Visible=true;
                labelPatternVerbSingle.Visible=true;
                labelPatternVerbMultiple.Visible=true;
            }else{ 
              

                textBoxPatternVerbAux1.Visible=false;
                textBoxPatternVerbAux2.Visible=false;
                textBoxPatternVerbAux3.Visible=false;
                textBoxPatternVerbAux4.Visible=false;
                textBoxPatternVerbAux5.Visible=false;
                textBoxPatternVerbAux6.Visible=false;
                tableLayoutPanelPatternVerbAux.Visible=false;
                labelPatternVerbAuxText.Visible=false;
                labelPatternVerbSingle.Visible=false;
                labelPatternVerbMultiple.Visible=false;
            }

            textBoxPatternVerbRoJ2.Text=CurrentPatternVerbTo.Imperative[0];
            textBoxPatternVerbRoM1.Text=CurrentPatternVerbTo.Imperative[1];
            textBoxPatternVerbRoM2.Text=CurrentPatternVerbTo.Imperative[2];
            tableLayoutPanelPatternVerbImperative.Visible=true;
            labelPatternVerbImperative.Visible=true;
            labelPatternVerbImperativeMultiple.Visible=true;
            labelPatternVerbImperativePeople.Visible=true;
            labelPatternVerbImperativeSingle.Visible=true;
                        
            if (CurrentPatternVerbTo.TypeShow==VerbTypeShow.Trpne || CurrentPatternVerbTo.TypeShow==VerbTypeShow.TrpneCinne || CurrentPatternVerbTo.TypeShow==VerbTypeShow.All || CurrentPatternVerbTo.TypeShow==VerbTypeShow.Unknown) {
                textBoxPatternVerbMtMzJ.Text=CurrentPatternVerbTo.PastPassive[0];
                textBoxPatternVerbMtMnJ.Text=CurrentPatternVerbTo.PastPassive[1];
                textBoxPatternVerbMtZeJ.Text=CurrentPatternVerbTo.PastPassive[2];
                textBoxPatternVerbMtStJ.Text=CurrentPatternVerbTo.PastPassive[3];
                textBoxPatternVerbMtMzM.Text=CurrentPatternVerbTo.PastPassive[4];
                textBoxPatternVerbMtMnM.Text=CurrentPatternVerbTo.PastPassive[5];
                textBoxPatternVerbMtZeM.Text=CurrentPatternVerbTo.PastPassive[6];
                textBoxPatternVerbMtStM.Text=CurrentPatternVerbTo.PastPassive[7];  
                
                tableLayoutPanelPatternVerbPastInactive.Visible=true;
                labelPatternVerbPastInactive.Visible=true;
                labelPatternVerbPastInactiveMultiple.Visible=true;
                labelPatternVerbPastInactivePeople.Visible=true;
                labelPatternVerbPastInactiveSingle.Visible=true;
            } else { 
                tableLayoutPanelPatternVerbPastInactive.Visible=false;
                labelPatternVerbPastInactive.Visible=false;
                labelPatternVerbPastInactiveMultiple.Visible=false;
                labelPatternVerbPastInactivePeople.Visible=false;
                labelPatternVerbPastInactiveSingle.Visible=false;
            }

            if (CurrentPatternVerbTo.TypeShow==VerbTypeShow.Cinne || CurrentPatternVerbTo.TypeShow==VerbTypeShow.TrpneCinne || CurrentPatternVerbTo.TypeShow==VerbTypeShow.All || CurrentPatternVerbTo.TypeShow==VerbTypeShow.Unknown) {
                textBoxPatternVerbMcMzJ.Text=CurrentPatternVerbTo.PastActive[0];
                textBoxPatternVerbMcMnJ.Text=CurrentPatternVerbTo.PastActive[1];
                textBoxPatternVerbMcZeJ.Text=CurrentPatternVerbTo.PastActive[2];
                textBoxPatternVerbMcStJ.Text=CurrentPatternVerbTo.PastActive[3];
                textBoxPatternVerbMcMzM.Text=CurrentPatternVerbTo.PastActive[4];
                textBoxPatternVerbMcMnM.Text=CurrentPatternVerbTo.PastActive[5];
                textBoxPatternVerbMcZeM.Text=CurrentPatternVerbTo.PastActive[6];
                textBoxPatternVerbMcStM.Text=CurrentPatternVerbTo.PastActive[7];

                tableLayoutPanelPatternVerbPastActive.Visible=true;
                labelVerbPatternPastActive.Visible=true;
                labelVerbPatternPastActiveMultiple.Visible=true;
                labelVerbPatternPastActivePeople.Visible=true;
                labelVerbPatternPastActiveSingle.Visible=true;
            }else{ 
                 tableLayoutPanelPatternVerbPastActive.Visible=false;
                labelVerbPatternPastActive.Visible=false;
                labelVerbPatternPastActiveMultiple.Visible=false;
                labelVerbPatternPastActivePeople.Visible=false;
                labelVerbPatternPastActiveSingle.Visible=false;
            }

            textBoxPatternVerbTr1.Text=CurrentPatternVerbTo.TransgressiveCont[0];
            textBoxPatternVerbTr2.Text=CurrentPatternVerbTo.TransgressiveCont[1];
            textBoxPatternVerbTr3.Text=CurrentPatternVerbTo.TransgressiveCont[2];
            textBoxPatternVerbTr4.Text=CurrentPatternVerbTo.TransgressivePast[0];
            textBoxPatternVerbTr5.Text=CurrentPatternVerbTo.TransgressivePast[1];
            textBoxPatternVerbTr6.Text=CurrentPatternVerbTo.TransgressivePast[2];

            tableLayoutPanelPatternVerbTransgressive.Visible=true;
            labelPatternVerbTransgressive.Visible=true; 
            PatternVerbTransgressiveContinous.Visible=true;
            PatternVerbTransgressivePast.Visible=true;
          
            textBoxPatternVerbTr1.Text=CurrentPatternVerbTo.TransgressiveCont[0];
            textBoxPatternVerbTr2.Text=CurrentPatternVerbTo.TransgressiveCont[1];
            textBoxPatternVerbTr3.Text=CurrentPatternVerbTo.TransgressiveCont[2];
            textBoxPatternVerbTr4.Text=CurrentPatternVerbTo.TransgressivePast[0];
            textBoxPatternVerbTr5.Text=CurrentPatternVerbTo.TransgressivePast[1];
            textBoxPatternVerbTr6.Text=CurrentPatternVerbTo.TransgressivePast[2];


            textBoxPatternVerbName.Visible=true;
            labelVerbPatterName.Visible=true;
            labelPatternVerbType.Visible=true;
          
            comboBoxPatternVerbType.Visible=true;
          
            
          
           
          
            textBoxPatternVerbInfinitive.Visible=true;
            labelPatternVerbInfinitive.Visible=true;
        }
         
        void PatternVerbToListBoxSetCurrent() {
            for (int indexCur=0; indexCur<itemsPatternVerbsToFiltered.Count; indexCur++) {
                if (itemsPatternVerbTo[indexCur]==CurrentPatternVerbTo) { 
                    int indexList=PatternVerbToListBox.SelectedIndex;
                    if (indexList==indexCur) return;
                    PatternVerbToListBox.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void PatternVerbToSaveCurrent() {
            if (CurrentPatternVerbTo==null) return;
                     
            CurrentPatternVerbTo.Name=textBoxPatternVerbName.Text;
            CurrentPatternVerbTo.Type=(VerbType)comboBoxPatternVerbType.SelectedIndex;
            CurrentPatternVerbTo.TypeShow=(VerbTypeShow)comboBoxPatternVerbShowType.SelectedIndex;
            CurrentPatternVerbTo.Infinitive=textBoxPatternVerbInfinitive.Text;

            CurrentPatternVerbTo.Continous[0]=textBoxPatternVerbPrJ1.Text;
            CurrentPatternVerbTo.Continous[1]=textBoxPatternVerbPrJ2.Text;
            CurrentPatternVerbTo.Continous[2]=textBoxPatternVerbPrJ3.Text;
            CurrentPatternVerbTo.Continous[3]=textBoxPatternVerbPrM1.Text;
            CurrentPatternVerbTo.Continous[4]=textBoxPatternVerbPrM2.Text;
            CurrentPatternVerbTo.Continous[5]=textBoxPatternVerbPrM3.Text;

            CurrentPatternVerbTo.Future[0]=textBoxPatternVerbBuJ1.Text;
            CurrentPatternVerbTo.Future[1]=textBoxPatternVerbBuJ2.Text;
            CurrentPatternVerbTo.Future[2]=textBoxPatternVerbBuJ3.Text;
            CurrentPatternVerbTo.Future[3]=textBoxPatternVerbBuM1.Text;
            CurrentPatternVerbTo.Future[4]=textBoxPatternVerbBuM2.Text;
            CurrentPatternVerbTo.Future[5]=textBoxPatternVerbBuM3.Text;

            CurrentPatternVerbTo.Imperative[0]=textBoxPatternVerbRoJ2.Text;
            CurrentPatternVerbTo.Imperative[1]=textBoxPatternVerbRoM1.Text;
            CurrentPatternVerbTo.Imperative[2]=textBoxPatternVerbRoM2.Text;
                        
            CurrentPatternVerbTo.PastPassive[0]=textBoxPatternVerbMtMzJ.Text;
            CurrentPatternVerbTo.PastPassive[1]=textBoxPatternVerbMtMnJ.Text;
            CurrentPatternVerbTo.PastPassive[2]=textBoxPatternVerbMtZeJ.Text;
            CurrentPatternVerbTo.PastPassive[3]=textBoxPatternVerbMtStJ.Text;
            CurrentPatternVerbTo.PastPassive[4]=textBoxPatternVerbMtMzM.Text;
            CurrentPatternVerbTo.PastPassive[5]=textBoxPatternVerbMtMnM.Text;
            CurrentPatternVerbTo.PastPassive[6]=textBoxPatternVerbMtZeM.Text;
            CurrentPatternVerbTo.PastPassive[7]=textBoxPatternVerbMtStM.Text;
            
            CurrentPatternVerbTo.PastActive[0]=textBoxPatternVerbMcMzJ.Text;
            CurrentPatternVerbTo.PastActive[1]=textBoxPatternVerbMcMnJ.Text;
            CurrentPatternVerbTo.PastActive[2]=textBoxPatternVerbMcZeJ.Text;
            CurrentPatternVerbTo.PastActive[3]=textBoxPatternVerbMcStJ.Text;
            CurrentPatternVerbTo.PastActive[4]=textBoxPatternVerbMcMzM.Text;
            CurrentPatternVerbTo.PastActive[5]=textBoxPatternVerbMcMnM.Text;
            CurrentPatternVerbTo.PastActive[6]=textBoxPatternVerbMcZeM.Text;
            CurrentPatternVerbTo.PastActive[7]=textBoxPatternVerbMcStM.Text;

            CurrentPatternVerbTo.TransgressiveCont[0]=textBoxPatternVerbTr1.Text;
            CurrentPatternVerbTo.TransgressiveCont[1]=textBoxPatternVerbTr2.Text;
            CurrentPatternVerbTo.TransgressiveCont[2]=textBoxPatternVerbTr3.Text;
            CurrentPatternVerbTo.TransgressivePast[0]=textBoxPatternVerbTr4.Text;
            CurrentPatternVerbTo.TransgressivePast[1]=textBoxPatternVerbTr5.Text;
            CurrentPatternVerbTo.TransgressivePast[2]=textBoxPatternVerbTr6.Text;

            CurrentPatternVerbTo.Auxiliary[0]=textBoxPatternVerbAux1.Text;
            CurrentPatternVerbTo.Auxiliary[1]=textBoxPatternVerbAux2.Text;
            CurrentPatternVerbTo.Auxiliary[2]=textBoxPatternVerbAux3.Text;
            CurrentPatternVerbTo.Auxiliary[3]=textBoxPatternVerbAux4.Text;
            CurrentPatternVerbTo.Auxiliary[4]=textBoxPatternVerbAux5.Text;
            CurrentPatternVerbTo.Auxiliary[5]=textBoxPatternVerbAux6.Text;
        } 
              
        void PatternVerbToSetNone(){ 
            textBoxPatternVerbName.Text="";

            textBoxPatternVerbPrJ1.Text="";
            textBoxPatternVerbPrJ2.Text="";
            textBoxPatternVerbPrJ3.Text="";
            textBoxPatternVerbPrM1.Text="";
            textBoxPatternVerbPrM2.Text="";
            textBoxPatternVerbPrM3.Text="";

            textBoxPatternVerbBuJ1.Text="";
            textBoxPatternVerbBuJ2.Text="";
            textBoxPatternVerbBuJ3.Text="";
            textBoxPatternVerbBuM1.Text="";
            textBoxPatternVerbBuM2.Text="";
            textBoxPatternVerbBuM3.Text="";

            textBoxPatternVerbRoJ2.Text="";
            textBoxPatternVerbRoM1.Text="";
            textBoxPatternVerbRoM2.Text="";
                        
            textBoxPatternVerbMtMzJ.Text="";
            textBoxPatternVerbMtMnJ.Text="";
            textBoxPatternVerbMtZeJ.Text="";
            textBoxPatternVerbMtStJ.Text="";
            textBoxPatternVerbMtMzM.Text="";
            textBoxPatternVerbMtMnM.Text="";
            textBoxPatternVerbMtZeM.Text="";
            textBoxPatternVerbMtStM.Text="";
            
            textBoxPatternVerbMcMzJ.Text="";
            textBoxPatternVerbMcMnJ.Text="";
            textBoxPatternVerbMcZeJ.Text="";
            textBoxPatternVerbMcStJ.Text="";
            textBoxPatternVerbMcMzM.Text="";
            textBoxPatternVerbMcMnM.Text="";
            textBoxPatternVerbMcZeM.Text="";
            textBoxPatternVerbMcStM.Text="";

            
            textBoxPatternVerbTr1.Text="";
            textBoxPatternVerbTr2.Text="";
            textBoxPatternVerbTr3.Text="";
            textBoxPatternVerbTr4.Text="";
            textBoxPatternVerbTr5.Text="";
            textBoxPatternVerbTr6.Text="";

            labelVerbPatterName.Visible=false;
            labelPatternVerbType.Visible=false;
            tableLayoutPanelPatternVerbContinous.Visible=false;
            labelPatternVerbContinous.Visible=false;
            labelPatternVerbContinousMultiple.Visible=false;
            labelPatternVerbContinousSingle.Visible=false;
            PatternVerbTransgressiveContinous.Visible=false;
            tableLayoutPanelPatternVerbFuture.Visible=false;
            labelPatternVerbFuture.Visible=false;
            labelPatternVerbFutureMultiple.Visible=false;
            labelPatternVerbFuturePeople.Visible=false;
            labelPatternVerbFutureSingle.Visible=false;
            comboBoxPatternVerbType.Visible=false;
            PatternVerbTransgressivePast.Visible=false;
            tableLayoutPanelPatternVerbImperative.Visible=false;
            labelPatternVerbImperative.Visible=false;
            labelPatternVerbImperativeMultiple.Visible=false;
            labelPatternVerbImperativePeople.Visible=false;
            labelPatternVerbImperativeSingle.Visible=false;
            labelPatternVerbContinousPeople.Visible=false;
            tableLayoutPanelPatternVerbPastInactive.Visible=false;
            labelPatternVerbPastInactive.Visible=false;
            labelPatternVerbPastInactiveMultiple.Visible=false;
            labelPatternVerbPastInactivePeople.Visible=false;
            labelPatternVerbPastInactiveSingle.Visible=false;
            textBoxPatternVerbName.Visible=false;
            tableLayoutPanelPatternVerbPastActive.Visible=false;
            labelVerbPatternPastActive.Visible=false;
            labelVerbPatternPastActiveMultiple.Visible=false;
            labelVerbPatternPastActivePeople.Visible=false;
            labelVerbPatternPastActiveSingle.Visible=false;

            tableLayoutPanelPatternVerbTransgressive.Visible=false;
            labelPatternVerbTransgressive.Visible=false; 
        }
        
        void PatternVerbToClear() { 
            PatternVerbToListBox.Items.Clear();
            PatternVerbToSetNone();
            itemsPatternVerbsToFiltered?.Clear();
            itemsPatternVerbTo?.Clear();
            CurrentPatternVerbTo=null;
        }
        #endregion

        #region Adverb
        void listBoxAdverb_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentAdverb();
            
            int index=listBoxAdverb.SelectedIndex;
            if (itemsAdverbs.Count==0) {
                SetNoneAdverb();
                return;
            }
            if (index>=itemsAdverbs.Count)    index=itemsAdverbs.Count-1;
            if (index<0) index=0;
           
            CurrentAdverb=itemsAdverbs[index];
            SetCurrentAdverb();
            SetListBoxAdverb();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void buttonAdverbAdd_Click(object sender, EventArgs e) {
            AddNewItemAdverb();
        }

        void buttonAdverbRemove_Click(object sender, EventArgs e) {
            RemoveItemAdverb(CurrentAdverb);
            TextBoxAdverbFilter_TextChanged(null, new EventArgs());
        }
        
        void TextBoxAdverbFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentAdverb();

            // Získej aktuální prvek
            ItemAdverb selectedId=null;
            if (listBoxAdverb.SelectedIndex!=-1) {
                selectedId=itemsAdverbsFiltered[listBoxAdverb.SelectedIndex];
            }
            
            AdverbRefreshFilteredList();

            listBoxAdverb.Items.Clear();
            for (int i=0; i<itemsAdverbsFiltered.Count; i++) {
                ItemAdverb item = itemsAdverbsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxAdverb.Items.Add(textToAdd);
            }

            //SetListBoxAdverb();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsAdverbsFiltered.Count; i++){
                    if (itemsAdverbsFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxAdverb.SelectedIndex=-1;
                    CurrentAdverb=null;
                } else listBoxAdverb.SelectedIndex=outIndex;
            } else {
                listBoxAdverb.SelectedIndex=-1;
                CurrentAdverb=null;
            }
            SetCurrentAdverb();
        }
            
        void RemoveCurrentAdverb(object sender, EventArgs e) {
            itemsAdverbs.Remove(CurrentAdverb);
        }

        void SetListBoxAdverb() { 
            string filter=textBoxAdverbFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxAdverb.SelectedIndex;
            listBoxAdverb.Items.Clear();
            for (int i=0; i<itemsAdverbsFiltered.Count; i++) {
                ItemAdverb item = itemsAdverbsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxAdverb.Items.Add(textToAdd);
            }

            if (index>=listBoxAdverb.Items.Count)index=listBoxAdverb.Items.Count-1;
            listBoxAdverb.SelectedIndex=index;
        }

        void AdverbRefreshFilteredList() {
            if (itemsAdverbsFiltered==null) itemsAdverbsFiltered=new List<ItemAdverb>();
            itemsAdverbsFiltered.Clear();
            string filter=textBoxAdverbFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsAdverbs.Count; i++) {
                    ItemAdverb item = itemsAdverbs[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsAdverbsFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsAdverbs.Count; i++) {
                    ItemAdverb item = itemsAdverbs[i];
                    itemsAdverbsFiltered.Add(item);
                }
            }
        }
               
        void AddNewItemAdverb() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentAdverb();

            var newItem=new ItemAdverb();
            // newItem.ID=itemsAdverbs.Count;
            itemsAdverbs.Add(newItem);
            CurrentAdverb=newItem;
            AdverbRefreshFilteredList();
            SetListBoxAdverb(); 
            ListBoxSetCurrentAdverb();
            SetCurrentAdverb(); 
            
            doingJob=false;
        }
   
        void RemoveItemAdverb(ItemAdverb item) { 
            Edited=true;
            ChangeCaptionText();
            itemsAdverbs.Remove(item);
            AdverbRefreshFilteredList();
            SetListBoxAdverb();
            SetCurrentAdverb();
        } 
           
        void SetCurrentAdverb(){
            if (itemsAdverbsFiltered.Count==0) {
                SetNoneAdverb();
                return;
            }

            int index=listBoxAdverb.SelectedIndex;
            if (index>=itemsAdverbsFiltered.Count) index=itemsAdverbsFiltered.Count-1;
            if (index<0) index=0;
            CurrentAdverb=itemsAdverbsFiltered[index];
            
            textBoxAdverbFrom.Visible=true;
            textBoxAdverbTo.Visible=true;
            labelAdverbFrom.Visible=true;
            labelAdverbTo.Visible=true; 

           textBoxAdverbFrom.Text= CurrentAdverb.From;
           textBoxAdverbTo.Text= CurrentAdverb.To;
      
        }
         
        void ListBoxSetCurrentAdverb() {
            for (int indexCur=0; indexCur<itemsAdverbsFiltered.Count; indexCur++) {
                if (itemsAdverbs[indexCur]==CurrentAdverb) { 
                    int indexList=listBoxAdverb.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxAdverb.SelectedIndex=indexCur;
                    break;
                }
            }
        }  

        //int GetListBoxSelectedIndexAdverb() {
        //    if (listBoxAdverbs.SelectedIndex==-1) return -1;

        //    string filter=textBoxFilterAdverb.Text;
        //    bool useFilter=filter=="" || filter=="*";

        //    if (useFilter) { 
        //        var item=itemsAdverbsFiltered[listBoxAdverbs.SelectedIndex];
        //        return item.ID;
        //        //for (int i=0; i<itemsAdverbs.Count; i++){
        //        //    if (i==item) return i;
        //        //}
        //    } else { 
        //        return listBoxAdverbs.SelectedIndex;
        //    }

        //    return -1;
        //}
        
        void SaveCurrentAdverb() {
            if (CurrentAdverb==null) return;
                     
            CurrentAdverb.From=textBoxAdverbFrom.Text;
            CurrentAdverb.To=textBoxAdverbTo.Text;
        } 
              
        void SetNoneAdverb(){ 
            textBoxAdverbFrom.Text="";
            textBoxAdverbTo.Text="";
            textBoxAdverbFrom.Visible=false;
            textBoxAdverbTo.Visible=false;
            labelAdverbFrom.Visible=false;
            labelAdverbTo.Visible=false;
        }
                
        void ClearAdverb(){ 
            listBoxAdverb.Items.Clear();
            SetNoneAdverb();
            itemsAdverbsFiltered?.Clear();
            itemsAdverbs?.Clear();
            CurrentAdverb=null;
        }
        #endregion
        
        #region Preposition
        void ListBoxPrepositions_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentPreposition();
            
            int index=listBoxPreposition.SelectedIndex;
            if (itemsPrepositions.Count==0) {
                SetNonePreposition();
                return;
            }
            if (index>=itemsPrepositions.Count) 
                index=itemsPrepositions.Count-1;
            if (index<0)
                index=0;
           
            CurrentPreposition=itemsPrepositions[index];
            SetCurrentPreposition();
            SetListBoxPreposition();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void ButtonPrepositionAdd_Click(object sender, EventArgs e) {
            AddNewItemPreposition();
        }

        void ButtonPrepositionRemove_Click(object sender, EventArgs e) {
            RemoveItemPreposition(CurrentPreposition);
            TextBoxPrepositionFilter_TextChanged(null, new EventArgs());
        }
        
        void TextBoxPrepositionFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentPreposition();

            // Získej aktuální prvek
            ItemPreposition selectedId=null;
            if (listBoxPreposition.SelectedIndex!=-1) {
                selectedId=itemsPrepositionsFiltered[listBoxPreposition.SelectedIndex];
            }
            
            PrepositionRefreshFilteredList();

            listBoxPreposition.Items.Clear();
            for (int i=0; i<itemsPrepositionsFiltered.Count; i++) {
                ItemPreposition item = itemsPrepositionsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPreposition.Items.Add(textToAdd);
            }

            //SetListBoxPronoun();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsPrepositionsFiltered.Count; i++){
                    if (itemsPrepositionsFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxPreposition.SelectedIndex=-1;
                    CurrentPreposition=null;
                } else listBoxPreposition.SelectedIndex=outIndex;
            } else {
                listBoxPreposition.SelectedIndex=-1;
                CurrentPreposition=null;
            }
            SetCurrentPreposition();
        }
            
        void RemoveCurrentPreposition(object sender, EventArgs e) {
            itemsPrepositions.Remove(CurrentPreposition);
        }

        void SetListBoxPreposition() { 
            string filter=textBoxPrepositionFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxPreposition.SelectedIndex;
            listBoxPreposition.Items.Clear();
            for (int i=0; i<itemsPrepositionsFiltered.Count; i++) {
                ItemPreposition item = itemsPrepositionsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxPreposition.Items.Add(textToAdd);
            }

            if (index>=listBoxPreposition.Items.Count)index=listBoxPreposition.Items.Count-1;
            listBoxPreposition.SelectedIndex=index;
        }

        void PrepositionRefreshFilteredList() {
            if (itemsPrepositionsFiltered==null) itemsPrepositionsFiltered=new List<ItemPreposition>();
            itemsPrepositionsFiltered.Clear();
            string filter=textBoxPrepositionFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsPrepositions.Count; i++) {
                    ItemPreposition item = itemsPrepositions[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsPrepositionsFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsPrepositions.Count; i++) {
                    ItemPreposition item = itemsPrepositions[i];
                    itemsPrepositionsFiltered.Add(item);
                }
            }
        }
               
        void AddNewItemPreposition() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentPreposition();

            var newItem=new ItemPreposition();
           // newItem.ID=itemsPronouns.Count;
            itemsPrepositions.Add(newItem);
            CurrentPreposition=newItem;
            PrepositionRefreshFilteredList();
            SetListBoxPreposition(); 
            ListBoxSetCurrentPreposition();
            SetCurrentPreposition(); 
            
            doingJob=false;
        }
   
        void RemoveItemPreposition(ItemPreposition item) { 
            Edited=true;
            ChangeCaptionText();
            itemsPrepositions.Remove(item);
            PrepositionRefreshFilteredList();
            SetListBoxPreposition();
            SetCurrentPreposition();
        } 
           
        void SetCurrentPreposition(){
            if (itemsPrepositionsFiltered.Count==0) {
                SetNonePreposition();
                return;
            }

            int index=listBoxPreposition.SelectedIndex;
            if (index>=itemsPrepositionsFiltered.Count) index=itemsPrepositionsFiltered.Count-1;
            if (index<0) index=0;
            CurrentPreposition=itemsPrepositionsFiltered[index];
            
            textBoxPrepositionFrom.Text=CurrentPreposition.From;
            textBoxPrepositionTo.Text=CurrentPreposition.To;
            textBoxPrepositionFall.Text=CurrentPreposition.Fall;

            textBoxPrepositionFrom.Visible=true;
            textBoxPrepositionTo.Visible=true;
            textBoxPrepositionFall.Visible=true;

            labelPrepositionFrom.Visible=true;
            labelPrepositionTo.Visible=true;
            labelPrepositionFall.Visible=true;
         
          //  ChangeTypePreposition(CurrentPreposition?.Type);
        }
         
        void ListBoxSetCurrentPreposition() {
            for (int indexCur=0; indexCur<itemsPrepositionsFiltered.Count; indexCur++) {
                if (itemsPrepositions[indexCur]==CurrentPreposition) { 
                    int indexList=listBoxPreposition.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxPreposition.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void SaveCurrentPreposition() {
            if (CurrentPreposition==null) return;

            CurrentPreposition.From =textBoxPrepositionFrom.Text;
            CurrentPreposition.To   =textBoxPrepositionTo.Text;
            CurrentPreposition.Fall =textBoxPrepositionFall.Text;
           
        } 
              
        void SetNonePreposition(){ 
            textBoxPrepositionFrom.Text="";
            textBoxPrepositionTo.Text="";
            textBoxPrepositionFall.Text="";

            textBoxPrepositionFrom.Visible=false;
            textBoxPrepositionTo.Visible=false;
            textBoxPrepositionFall.Visible=false;

            labelPrepositionFrom.Visible=false;
            labelPrepositionTo.Visible=false;
            labelPrepositionFall.Visible=false;
        }
                
        void ClearPreposition() { 
            listBoxPreposition.Items.Clear();
            SetNonePreposition();
            itemsPrepositionsFiltered?.Clear();
            itemsPrepositions?.Clear();
            CurrentPreposition=null;
        }
        #endregion

        #region Conjunction
        void ListBoxConjunction_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentConjunction();
            
            int index=listBoxConjunction.SelectedIndex;
            if (itemsConjunctions.Count==0) {
                SetNoneConjunction();
                return;
            }
            if (index>=itemsConjunctions.Count)    index=itemsConjunctions.Count-1;
            if (index<0) index=0;
           
            CurrentConjunction=itemsConjunctions[index];
            SetCurrentConjunction();
            SetListBoxConjunction();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void ButtonConjunctionAdd_Click(object sender, EventArgs e) {
            AddNewItemConjunction();
        }

        void ButtonConjunctionRemove_Click(object sender, EventArgs e) {
            RemoveItemConjunction(CurrentConjunction);
            TextBoxConjunctionFilter_TextChanged(null, new EventArgs());
        }
        
        void TextBoxConjunctionFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentConjunction();

            // Získej aktuální prvek
            ItemConjunction selectedId=null;
            if (listBoxConjunction.SelectedIndex!=-1) {
                selectedId=itemsConjunctionsFiltered[listBoxConjunction.SelectedIndex];
            }
            
            ConjunctionRefreshFilteredList();

            listBoxConjunction.Items.Clear();
            for (int i=0; i<itemsConjunctionsFiltered.Count; i++) {
                ItemConjunction item = itemsConjunctionsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxConjunction.Items.Add(textToAdd);
            }

            //SetListBoxConjunction();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsConjunctionsFiltered.Count; i++){
                    if (itemsConjunctionsFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxConjunction.SelectedIndex=-1;
                    CurrentConjunction=null;
                } else listBoxConjunction.SelectedIndex=outIndex;
            } else {
                listBoxConjunction.SelectedIndex=-1;
                CurrentConjunction=null;
            }
            SetCurrentConjunction();
        }
            
        void RemoveCurrentConjunction(object sender, EventArgs e) {
            itemsConjunctions.Remove(CurrentConjunction);
        }

        void SetListBoxConjunction() { 
            string filter=textBoxConjunctionFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxConjunction.SelectedIndex;
            listBoxConjunction.Items.Clear();
            for (int i=0; i<itemsConjunctionsFiltered.Count; i++) {
                ItemConjunction item = itemsConjunctionsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxConjunction.Items.Add(textToAdd);
            }

            if (index>=listBoxConjunction.Items.Count)index=listBoxConjunction.Items.Count-1;
            listBoxConjunction.SelectedIndex=index;
        }

        void ConjunctionRefreshFilteredList() {
            if (itemsConjunctionsFiltered==null) itemsConjunctionsFiltered=new List<ItemConjunction>();
            itemsConjunctionsFiltered.Clear();
            string filter=textBoxConjunctionFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsConjunctions.Count; i++) {
                    ItemConjunction item = itemsConjunctions[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsConjunctionsFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsConjunctions.Count; i++) {
                    ItemConjunction item = itemsConjunctions[i];
                    itemsConjunctionsFiltered.Add(item);
                }
            }
        }
               
        void AddNewItemConjunction() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentConjunction();

            var newItem=new ItemConjunction();
            // newItem.ID=itemsConjunctions.Count;
            itemsConjunctions.Add(newItem);
            CurrentConjunction=newItem;
            ConjunctionRefreshFilteredList();
            SetListBoxConjunction(); 
            ListBoxSetCurrentConjunction();
            SetCurrentConjunction(); 
            
            doingJob=false;
        }
   
        void RemoveItemConjunction(ItemConjunction item) { 
            Edited=true;
            ChangeCaptionText();
            itemsConjunctions.Remove(item);
            ConjunctionRefreshFilteredList();
            SetListBoxConjunction();
            SetCurrentConjunction();
        } 
           
        void SetCurrentConjunction(){
            if (itemsConjunctionsFiltered.Count==0) {
                SetNoneConjunction();
                return;
            }

            int index=listBoxConjunction.SelectedIndex;
            if (index>=itemsConjunctionsFiltered.Count) index=itemsConjunctionsFiltered.Count-1;
            if (index<0) index=0;
            CurrentConjunction=itemsConjunctionsFiltered[index];
            
            textBoxConjunctionFrom.Visible=true;
            textBoxConjunctionTo.Visible=true;
            labelConjunctionFrom.Visible=true;
            labelConjunctionTo.Visible=true; 

           textBoxConjunctionFrom.Text= CurrentConjunction.From;
           textBoxConjunctionTo.Text= CurrentConjunction.To;
      
        }
         
        void ListBoxSetCurrentConjunction() {
            for (int indexCur=0; indexCur<itemsConjunctionsFiltered.Count; indexCur++) {
                if (itemsConjunctions[indexCur]==CurrentConjunction) { 
                    int indexList=listBoxConjunction.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxConjunction.SelectedIndex=indexCur;
                    break;
                }
            }
        }  

        //int GetListBoxSelectedIndexConjunction() {
        //    if (listBoxConjunctions.SelectedIndex==-1) return -1;

        //    string filter=textBoxFilterConjunction.Text;
        //    bool useFilter=filter=="" || filter=="*";

        //    if (useFilter) { 
        //        var item=itemsConjunctionsFiltered[listBoxConjunctions.SelectedIndex];
        //        return item.ID;
        //        //for (int i=0; i<itemsConjunctions.Count; i++){
        //        //    if (i==item) return i;
        //        //}
        //    } else { 
        //        return listBoxConjunctions.SelectedIndex;
        //    }

        //    return -1;
        //}
        
        void SaveCurrentConjunction() {
            if (CurrentConjunction==null) return;
                     
            CurrentConjunction.From=textBoxConjunctionFrom.Text;
            CurrentConjunction.To=textBoxConjunctionTo.Text;
        } 
              
        void SetNoneConjunction(){ 
            textBoxConjunctionFrom.Text="";
            textBoxConjunctionTo.Text="";
            textBoxConjunctionFrom.Visible=false;
            textBoxConjunctionTo.Visible=false;
            labelConjunctionFrom.Visible=false;
            labelConjunctionTo.Visible=false;
        }
                
        void ClearConjunction(){ 
            listBoxConjunction.Items.Clear();
            SetNoneConjunction();
            itemsConjunctionsFiltered?.Clear();
            itemsConjunctions?.Clear();
            CurrentConjunction=null;
        }
        #endregion
 
        #region Particle
        void listBoxParticle_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentParticle();
            
            int index=listBoxParticle.SelectedIndex;
            if (itemsParticles.Count==0) {
                SetNoneParticle();
                return;
            }
            if (index>=itemsParticles.Count)    index=itemsParticles.Count-1;
            if (index<0) index=0;
           
            CurrentParticle=itemsParticles[index];
            SetCurrentParticle();
            SetListBoxParticle();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void buttonParticleAdd_Click(object sender, EventArgs e) {
            AddNewItemParticle();
        }

        void buttonParticleRemove_Click(object sender, EventArgs e) {
            RemoveItemParticle(CurrentParticle);
            TextBoxParticleFilter_TextChanged(null, new EventArgs());
        }
        
        void TextBoxParticleFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentParticle();

            // Získej aktuální prvek
            ItemParticle selectedId=null;
            if (listBoxParticle.SelectedIndex!=-1) {
                selectedId=itemsParticlesFiltered[listBoxParticle.SelectedIndex];
            }
            
            ParticleRefreshFilteredList();

            listBoxParticle.Items.Clear();
            for (int i=0; i<itemsParticlesFiltered.Count; i++) {
                ItemParticle item = itemsParticlesFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxParticle.Items.Add(textToAdd);
            }

            //SetListBoxParticle();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsParticlesFiltered.Count; i++){
                    if (itemsParticlesFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxParticle.SelectedIndex=-1;
                    CurrentParticle=null;
                } else listBoxParticle.SelectedIndex=outIndex;
            } else {
                listBoxParticle.SelectedIndex=-1;
                CurrentParticle=null;
            }
            SetCurrentParticle();
        }
            
        void RemoveCurrentParticle(object sender, EventArgs e) {
            itemsParticles.Remove(CurrentParticle);
        }

        void SetListBoxParticle() { 
            string filter=textBoxParticleFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxParticle.SelectedIndex;
            listBoxParticle.Items.Clear();
            for (int i=0; i<itemsParticlesFiltered.Count; i++) {
                ItemParticle item = itemsParticlesFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxParticle.Items.Add(textToAdd);
            }

            if (index>=listBoxParticle.Items.Count)index=listBoxParticle.Items.Count-1;
            listBoxParticle.SelectedIndex=index;
        }

        void ParticleRefreshFilteredList() {
            if (itemsParticlesFiltered==null) itemsParticlesFiltered=new List<ItemParticle>();
            itemsParticlesFiltered.Clear();
            string filter=textBoxParticleFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsParticles.Count; i++) {
                    ItemParticle item = itemsParticles[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsParticlesFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsParticles.Count; i++) {
                    ItemParticle item = itemsParticles[i];
                    itemsParticlesFiltered.Add(item);
                }
            }
        }
               
        void AddNewItemParticle() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentParticle();

            var newItem=new ItemParticle();
            // newItem.ID=itemsParticles.Count;
            itemsParticles.Add(newItem);
            CurrentParticle=newItem;
            ParticleRefreshFilteredList();
            SetListBoxParticle(); 
            ListBoxSetCurrentParticle();
            SetCurrentParticle(); 
            
            doingJob=false;
        }
   
        void RemoveItemParticle(ItemParticle item) { 
            Edited=true;
            ChangeCaptionText();
            itemsParticles.Remove(item);
            ParticleRefreshFilteredList();
            SetListBoxParticle();
            SetCurrentParticle();
        } 
           
        void SetCurrentParticle(){
            if (itemsParticlesFiltered.Count==0) {
                SetNoneParticle();
                return;
            }

            int index=listBoxParticle.SelectedIndex;
            if (index>=itemsParticlesFiltered.Count) index=itemsParticlesFiltered.Count-1;
            if (index<0) index=0;
            CurrentParticle=itemsParticlesFiltered[index];
            
            textBoxParticleFrom.Visible=true;
            textBoxParticleTo.Visible=true;
            labelParticleFrom.Visible=true;
            labelParticleTo.Visible=true; 

           textBoxParticleFrom.Text= CurrentParticle.From;
           textBoxParticleTo.Text= CurrentParticle.To;
      
        }
         
        void ListBoxSetCurrentParticle() {
            for (int indexCur=0; indexCur<itemsParticlesFiltered.Count; indexCur++) {
                if (itemsParticles[indexCur]==CurrentParticle) { 
                    int indexList=listBoxParticle.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxParticle.SelectedIndex=indexCur;
                    break;
                }
            }
        }  

        //int GetListBoxSelectedIndexParticle() {
        //    if (listBoxParticles.SelectedIndex==-1) return -1;

        //    string filter=textBoxFilterParticle.Text;
        //    bool useFilter=filter=="" || filter=="*";

        //    if (useFilter) { 
        //        var item=itemsParticlesFiltered[listBoxParticles.SelectedIndex];
        //        return item.ID;
        //        //for (int i=0; i<itemsParticles.Count; i++){
        //        //    if (i==item) return i;
        //        //}
        //    } else { 
        //        return listBoxParticles.SelectedIndex;
        //    }

        //    return -1;
        //}
        
        void SaveCurrentParticle() {
            if (CurrentParticle==null) return;
                     
            CurrentParticle.From=textBoxParticleFrom.Text;
            CurrentParticle.To=textBoxParticleTo.Text;
        } 
              
        void SetNoneParticle(){ 
            textBoxParticleFrom.Text="";
            textBoxParticleTo.Text="";
            textBoxParticleFrom.Visible=false;
            textBoxParticleTo.Visible=false;
            labelParticleFrom.Visible=false;
            labelParticleTo.Visible=false;
        }
                
        void ClearParticle(){ 
            listBoxParticle.Items.Clear();
            SetNoneParticle();
            itemsParticlesFiltered?.Clear();
            itemsParticles?.Clear();
            CurrentParticle=null;
        }
        #endregion

        #region Interjection
        void listBoxInterjection_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentInterjection();
            
            int index=listBoxInterjection.SelectedIndex;
            if (itemsInterjections.Count==0) {
                SetNoneInterjection();
                return;
            }
            if (index>=itemsInterjections.Count)    index=itemsInterjections.Count-1;
            if (index<0) index=0;
           
            CurrentInterjection=itemsInterjections[index];
            SetCurrentInterjection();
            SetListBoxInterjection();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void ButtonInterjectionAdd_Click(object sender, EventArgs e) {
            AddNewItemInterjection();
        }

        void ButtonInterjectionRemove_Click(object sender, EventArgs e) {
            RemoveItemInterjection(CurrentInterjection);
            TextBoxInterjectionFilter_TextChanged(null, new EventArgs());
        }
        
        void TextBoxInterjectionFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentInterjection();

            // Získej aktuální prvek
            ItemInterjection selectedId=null;
            if (listBoxInterjection.SelectedIndex!=-1) {
                selectedId=itemsInterjectionsFiltered[listBoxInterjection.SelectedIndex];
            }
            
            InterjectionRefreshFilteredList();

            listBoxInterjection.Items.Clear();
            for (int i=0; i<itemsInterjectionsFiltered.Count; i++) {
                ItemInterjection item = itemsInterjectionsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxInterjection.Items.Add(textToAdd);
            }

            //SetListBoxInterjection();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsInterjectionsFiltered.Count; i++){
                    if (itemsInterjectionsFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxInterjection.SelectedIndex=-1;
                    CurrentInterjection=null;
                } else listBoxInterjection.SelectedIndex=outIndex;
            } else {
                listBoxInterjection.SelectedIndex=-1;
                CurrentInterjection=null;
            }
            SetCurrentInterjection();
        }
            
        void RemoveCurrentInterjection(object sender, EventArgs e) {
            itemsInterjections.Remove(CurrentInterjection);
        }

        void SetListBoxInterjection() { 
            string filter=textBoxInterjectionFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxInterjection.SelectedIndex;
            listBoxInterjection.Items.Clear();
            for (int i=0; i<itemsInterjectionsFiltered.Count; i++) {
                ItemInterjection item = itemsInterjectionsFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxInterjection.Items.Add(textToAdd);
            }

            if (index>=listBoxInterjection.Items.Count)index=listBoxInterjection.Items.Count-1;
            listBoxInterjection.SelectedIndex=index;
        }

        void InterjectionRefreshFilteredList() {
            if (itemsInterjectionsFiltered==null) itemsInterjectionsFiltered=new List<ItemInterjection>();
            itemsInterjectionsFiltered.Clear();
            string filter=textBoxInterjectionFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsInterjections.Count; i++) {
                    ItemInterjection item = itemsInterjections[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsInterjectionsFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsInterjections.Count; i++) {
                    ItemInterjection item = itemsInterjections[i];
                    itemsInterjectionsFiltered.Add(item);
                }
            }
        }
               
        void AddNewItemInterjection() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentInterjection();

            var newItem=new ItemInterjection();
            // newItem.ID=itemsInterjections.Count;
            itemsInterjections.Add(newItem);
            CurrentInterjection=newItem;
            InterjectionRefreshFilteredList();
            SetListBoxInterjection(); 
            ListBoxSetCurrentInterjection();
            SetCurrentInterjection(); 
            
            doingJob=false;
        }
   
        void RemoveItemInterjection(ItemInterjection item) { 
            Edited=true;
            ChangeCaptionText();
            itemsInterjections.Remove(item);
            InterjectionRefreshFilteredList();
            SetListBoxInterjection();
            SetCurrentInterjection();
        } 
           
        void SetCurrentInterjection(){
            if (itemsInterjectionsFiltered.Count==0) {
                SetNoneInterjection();
                return;
            }

            int index=listBoxInterjection.SelectedIndex;
            if (index>=itemsInterjectionsFiltered.Count) index=itemsInterjectionsFiltered.Count-1;
            if (index<0) index=0;
            CurrentInterjection=itemsInterjectionsFiltered[index];
            
            textBoxInterjectionFrom.Visible=true;
            textBoxInterjectionTo.Visible=true;
            labelInterjectionFrom.Visible=true;
            labelInterjectionTo.Visible=true; 

           textBoxInterjectionFrom.Text= CurrentInterjection.From;
           textBoxInterjectionTo.Text= CurrentInterjection.To;
      
        }
         
        void ListBoxSetCurrentInterjection() {
            for (int indexCur=0; indexCur<itemsInterjectionsFiltered.Count; indexCur++) {
                if (itemsInterjections[indexCur]==CurrentInterjection) { 
                    int indexList=listBoxInterjection.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxInterjection.SelectedIndex=indexCur;
                    break;
                }
            }
        }  

        //int GetListBoxSelectedIndexInterjection() {
        //    if (listBoxInterjections.SelectedIndex==-1) return -1;

        //    string filter=textBoxFilterInterjection.Text;
        //    bool useFilter=filter=="" || filter=="*";

        //    if (useFilter) { 
        //        var item=itemsInterjectionsFiltered[listBoxInterjections.SelectedIndex];
        //        return item.ID;
        //        //for (int i=0; i<itemsInterjections.Count; i++){
        //        //    if (i==item) return i;
        //        //}
        //    } else { 
        //        return listBoxInterjections.SelectedIndex;
        //    }

        //    return -1;
        //}
        
        void SaveCurrentInterjection() {
            if (CurrentInterjection==null) return;
                     
            CurrentInterjection.From=textBoxInterjectionFrom.Text;
            CurrentInterjection.To=textBoxInterjectionTo.Text;
        } 
              
        void SetNoneInterjection(){ 
            textBoxInterjectionFrom.Text="";
            textBoxInterjectionTo.Text="";
            textBoxInterjectionFrom.Visible=false;
            textBoxInterjectionTo.Visible=false;
            labelInterjectionFrom.Visible=false;
            labelInterjectionTo.Visible=false;
        }
                
        void ClearInterjection(){ 
            listBoxInterjection.Items.Clear();
            SetNoneInterjection();
            itemsInterjectionsFiltered?.Clear();
            itemsInterjections?.Clear();
            CurrentInterjection=null;
        }
        #endregion
                
        #region ReplaceS
        void ListBoxReplaceSs_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentReplaceS();
            
            int index=listBoxReplaceS.SelectedIndex;
            if (itemsReplaceS.Count==0) {
                SetNoneReplaceS();
                return;
            }
            if (index>=itemsReplaceS.Count) 
                index=itemsReplaceS.Count-1;
            if (index<0)
                index=0;
           
            CurrentReplaceS=itemsReplaceS[index];
            SetCurrentReplaceS();
            SetListBoxReplaceS();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void ButtonReplaceSAdd_Click(object sender, EventArgs e) {
            AddNewItemReplaceS();
        }

        void ButtonReplaceSRemove_Click(object sender, EventArgs e) {
            RemoveItemReplaceS(CurrentReplaceS);
            TextBoxReplaceSFilter_TextChanged(null, new EventArgs());
        }
        
        void TextBoxReplaceSFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentReplaceS();

            // Získej aktuální prvek
            ItemReplaceS selectedId=null;
            if (listBoxReplaceS.SelectedIndex!=-1) {
                selectedId=itemsReplaceSFiltered[listBoxReplaceS.SelectedIndex];
            }
            
            ReplaceSRefreshFilteredList();

            listBoxReplaceS.Items.Clear();
            for (int i=0; i<itemsReplaceSFiltered.Count; i++) {
                ItemReplaceS item = itemsReplaceSFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxReplaceS.Items.Add(textToAdd);
            }

            //SetListBoxPronoun();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsReplaceSFiltered.Count; i++){
                    if (itemsReplaceSFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxReplaceS.SelectedIndex=-1;
                    CurrentReplaceS=null;
                } else listBoxReplaceS.SelectedIndex=outIndex;
            } else {
                listBoxReplaceS.SelectedIndex=-1;
                CurrentReplaceS=null;
            }
            SetCurrentReplaceS();
        }
            
        void RemoveCurrentReplaceS(object sender, EventArgs e) {
            itemsReplaceS.Remove(CurrentReplaceS);
        }

        void SetListBoxReplaceS() { 
            string filter=textBoxReplaceSFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxReplaceS.SelectedIndex;
            listBoxReplaceS.Items.Clear();
            for (int i=0; i<itemsReplaceSFiltered.Count; i++) {
                ItemReplaceS item = itemsReplaceSFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxReplaceS.Items.Add(textToAdd);
            }

            if (index>=listBoxReplaceS.Items.Count)index=listBoxReplaceS.Items.Count-1;
            listBoxReplaceS.SelectedIndex=index;
        }

        void ReplaceSRefreshFilteredList() {
            if (itemsReplaceSFiltered==null) itemsReplaceSFiltered=new List<ItemReplaceS>();
            itemsReplaceSFiltered.Clear();
            string filter=textBoxReplaceSFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsReplaceS.Count; i++) {
                    ItemReplaceS item = itemsReplaceS[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsReplaceSFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsReplaceS.Count; i++) {
                    ItemReplaceS item = itemsReplaceS[i];
                    itemsReplaceSFiltered.Add(item);
                }
            }
        }
               
        void AddNewItemReplaceS() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentReplaceS();

            var newItem=new ItemReplaceS();
           // newItem.ID=itemsPronouns.Count;
            itemsReplaceS.Add(newItem);
            CurrentReplaceS=newItem;
            ReplaceSRefreshFilteredList();
            SetListBoxReplaceS(); 
            ListBoxSetCurrentReplaceS();
            SetCurrentReplaceS(); 
            
            doingJob=false;
        }
   
        void RemoveItemReplaceS(ItemReplaceS item) { 
            Edited=true;
            ChangeCaptionText();
            itemsReplaceS.Remove(item);
            ReplaceSRefreshFilteredList();
            SetListBoxReplaceS();
            SetCurrentReplaceS();
        } 
           
        void SetCurrentReplaceS(){
            if (itemsReplaceSFiltered.Count==0) {
                SetNoneReplaceS();
                return;
            }

            int index=listBoxReplaceS.SelectedIndex;
            if (index>=itemsReplaceSFiltered.Count) index=itemsReplaceSFiltered.Count-1;
            if (index<0) index=0;
            CurrentReplaceS=itemsReplaceSFiltered[index];
            
            textBoxReplaceSFrom.Text=CurrentReplaceS.From;
            textBoxReplaceSTo.Text=CurrentReplaceS.To;
          //  textBoxReplaceSFall.Text=CurrentReplaceS.Fall;

            textBoxReplaceSFrom.Visible=true;
            textBoxReplaceSTo.Visible=true;
            //textBoxReplaceSFall.Visible=true;

            labelReplaceSFrom.Visible=true;
            labelReplaceSTo.Visible=true;
            //labelReplaceSFall.Visible=true;
         
          //  ChangeTypeReplaceS(CurrentReplaceS?.Type);
        }
         
        void ListBoxSetCurrentReplaceS() {
            for (int indexCur=0; indexCur<itemsReplaceSFiltered.Count; indexCur++) {
                if (itemsReplaceS[indexCur]==CurrentReplaceS) { 
                    int indexList=listBoxReplaceS.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxReplaceS.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void SaveCurrentReplaceS() {
            if (CurrentReplaceS==null) return;

            CurrentReplaceS.From =textBoxReplaceSFrom.Text;
            CurrentReplaceS.To   =textBoxReplaceSTo.Text;
          //  CurrentReplaceS.Fall =textBoxReplaceSFall.Text;
           
        } 
              
        void SetNoneReplaceS(){ 
            textBoxReplaceSFrom.Text="";
            textBoxReplaceSTo.Text="";
          //  textBoxReplaceSFall.Text="";

            textBoxReplaceSFrom.Visible=false;
            textBoxReplaceSTo.Visible=false;
          //  textBoxReplaceSFall.Visible=false;

            labelReplaceSFrom.Visible=false;
            labelReplaceSTo.Visible=false;
          //  labelReplaceSFall.Visible=false;
        }
                
        void ClearReplaceS() { 
            listBoxReplaceS.Items.Clear();
            SetNoneReplaceS();
            itemsReplaceSFiltered?.Clear();
            itemsReplaceS?.Clear();
            CurrentReplaceS=null;
        }
        #endregion

        #region ReplaceG
        void ListBoxReplaceGs_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentReplaceG();
            
            int index=listBoxReplaceG.SelectedIndex;
            if (itemsReplaceG.Count==0) {
                SetNoneReplaceG();
                return;
            }
            if (index>=itemsReplaceG.Count) 
                index=itemsReplaceG.Count-1;
            if (index<0)
                index=0;
           
            CurrentReplaceG=itemsReplaceG[index];
            SetCurrentReplaceG();
            SetListBoxReplaceG();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void ButtonReplaceGAdd_Click(object sender, EventArgs e) {
            AddNewItemReplaceG();
        }

        void ButtonReplaceGRemove_Click(object sender, EventArgs e) {
            RemoveItemReplaceG(CurrentReplaceG);
            TextBoxReplaceGFilter_TextChanged(null, new EventArgs());
        }
        
        void TextBoxReplaceGFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentReplaceG();

            // Získej aktuální prvek
            ItemReplaceG selectedId=null;
            if (listBoxReplaceG.SelectedIndex!=-1) {
                selectedId=itemsReplaceGFiltered[listBoxReplaceG.SelectedIndex];
            }
            
            ReplaceGRefreshFilteredList();

            listBoxReplaceG.Items.Clear();
            for (int i=0; i<itemsReplaceGFiltered.Count; i++) {
                ItemReplaceG item = itemsReplaceGFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxReplaceG.Items.Add(textToAdd);
            }

            //SetListBoxPronoun();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsReplaceGFiltered.Count; i++){
                    if (itemsReplaceGFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxReplaceG.SelectedIndex=-1;
                    CurrentReplaceG=null;
                } else listBoxReplaceG.SelectedIndex=outIndex;
            } else {
                listBoxReplaceG.SelectedIndex=-1;
                CurrentReplaceG=null;
            }
            SetCurrentReplaceG();
        }
            
        void RemoveCurrentReplaceG(object sender, EventArgs e) {
            itemsReplaceG.Remove(CurrentReplaceG);
        }

        void SetListBoxReplaceG() { 
            string filter=textBoxReplaceGFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxReplaceG.SelectedIndex;
            listBoxReplaceG.Items.Clear();
            for (int i=0; i<itemsReplaceGFiltered.Count; i++) {
                ItemReplaceG item = itemsReplaceGFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxReplaceG.Items.Add(textToAdd);
            }

            if (index>=listBoxReplaceG.Items.Count)index=listBoxReplaceG.Items.Count-1;
            listBoxReplaceG.SelectedIndex=index;
        }

        void ReplaceGRefreshFilteredList() {
            if (itemsReplaceGFiltered==null) itemsReplaceGFiltered=new List<ItemReplaceG>();
            itemsReplaceGFiltered.Clear();
            string filter=textBoxReplaceGFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsReplaceG.Count; i++) {
                    ItemReplaceG item = itemsReplaceG[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsReplaceGFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsReplaceG.Count; i++) {
                    ItemReplaceG item = itemsReplaceG[i];
                    itemsReplaceGFiltered.Add(item);
                }
            }
        }
               
        void AddNewItemReplaceG() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentReplaceG();

            var newItem=new ItemReplaceG();
           // newItem.ID=itemsPronouns.Count;
            itemsReplaceG.Add(newItem);
            CurrentReplaceG=newItem;
            ReplaceGRefreshFilteredList();
            SetListBoxReplaceG(); 
            ListBoxSetCurrentReplaceG();
            SetCurrentReplaceG(); 
            
            doingJob=false;
        }
   
        void RemoveItemReplaceG(ItemReplaceG item) { 
            Edited=true;
            ChangeCaptionText();
            itemsReplaceG.Remove(item);
            ReplaceGRefreshFilteredList();
            SetListBoxReplaceG();
            SetCurrentReplaceG();
        } 
           
        void SetCurrentReplaceG(){
            if (itemsReplaceGFiltered.Count==0) {
                SetNoneReplaceG();
                return;
            }

            int index=listBoxReplaceG.SelectedIndex;
            if (index>=itemsReplaceGFiltered.Count) index=itemsReplaceGFiltered.Count-1;
            if (index<0) index=0;
            CurrentReplaceG=itemsReplaceGFiltered[index];
            
            textBoxReplaceGFrom.Text=CurrentReplaceG.From;
            textBoxReplaceGTo.Text=CurrentReplaceG.To;
           // textBoxReplaceGFall.Text=CurrentReplaceG.Fall;

            textBoxReplaceGFrom.Visible=true;
            textBoxReplaceGTo.Visible=true;
            //textBoxReplaceGFall.Visible=true;

            labelReplaceGFrom.Visible=true;
            labelReplaceGTo.Visible=true;
           // labelReplaceGFall.Visible=true;
         
          //  ChangeTypeReplaceG(CurrentReplaceG?.Type);
        }
         
        void ListBoxSetCurrentReplaceG() {
            for (int indexCur=0; indexCur<itemsReplaceGFiltered.Count; indexCur++) {
                if (itemsReplaceG[indexCur]==CurrentReplaceG) { 
                    int indexList=listBoxReplaceG.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxReplaceG.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void SaveCurrentReplaceG() {
            if (CurrentReplaceG==null) return;

            CurrentReplaceG.From =textBoxReplaceGFrom.Text;
            CurrentReplaceG.To   =textBoxReplaceGTo.Text;
          //  CurrentReplaceG.Fall =textBoxReplaceGFall.Text;
           
        } 
              
        void SetNoneReplaceG(){ 
            textBoxReplaceGFrom.Text="";
            textBoxReplaceGTo.Text="";
         //   textBoxReplaceGFall.Text="";

            textBoxReplaceGFrom.Visible=false;
            textBoxReplaceGTo.Visible=false;
          //  textBoxReplaceGFall.Visible=false;

            labelReplaceGFrom.Visible=false;
            labelReplaceGTo.Visible=false;
           // labelReplaceGFall.Visible=false;
        }
                
        void ClearReplaceG() { 
            listBoxReplaceG.Items.Clear();
            SetNoneReplaceG();
            itemsReplaceGFiltered?.Clear();
            itemsReplaceG?.Clear();
            CurrentReplaceG=null;
        }
        #endregion

        #region ReplaceE
        void ListBoxReplaceEs_SelectedIndexChanged(object sender, EventArgs e) {
            if (doingJob) return;
            doingJob=true;
            SaveCurrentReplaceE();
            
            int index=listBoxReplaceE.SelectedIndex;
            if (itemsReplaceE.Count==0) {
                SetNoneReplaceE();
                return;
            }
            if (index>=itemsReplaceE.Count) 
                index=itemsReplaceE.Count-1;
            if (index<0)
                index=0;
           
            CurrentReplaceE=itemsReplaceE[index];
            SetCurrentReplaceE();
            SetListBoxReplaceE();
          //  SetCurrent();
            doingJob=false;
        }  
        
        void ButtonReplaceEAdd_Click(object sender, EventArgs e) {
            AddNewItemReplaceE();
        }

        void ButtonReplaceERemove_Click(object sender, EventArgs e) {
            RemoveItemReplaceE(CurrentReplaceE);
            TextBoxReplaceEFilter_TextChanged(null, new EventArgs());
        }
        
        void TextBoxReplaceEFilter_TextChanged(object sender, EventArgs e) {
            SaveCurrentReplaceE();

            // Získej aktuální prvek
            ItemReplaceE selectedId=null;
            if (listBoxReplaceE.SelectedIndex!=-1) {
                selectedId=itemsReplaceEFiltered[listBoxReplaceE.SelectedIndex];
            }
            
            ReplaceERefreshFilteredList();

            listBoxReplaceE.Items.Clear();
            for (int i=0; i<itemsReplaceEFiltered.Count; i++) {
                ItemReplaceE item = itemsReplaceEFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxReplaceE.Items.Add(textToAdd);
            }

            //SetListBoxPronoun();

            // Zkus nasadit aktuální prvek, když ne tak +1
            if (selectedId!=null){ 
                int outIndex=-1;
                for (int i=0; i<itemsReplaceEFiltered.Count; i++){
                    if (itemsReplaceEFiltered[i]==selectedId){ 
                        outIndex=i;
                        break;    
                    }    
                }

                if (outIndex==-1){
                    listBoxReplaceE.SelectedIndex=-1;
                    CurrentReplaceE=null;
                } else listBoxReplaceE.SelectedIndex=outIndex;
            } else {
                listBoxReplaceE.SelectedIndex=-1;
                CurrentReplaceE=null;
            }
            SetCurrentReplaceE();
        }
            
        void RemoveCurrentReplaceE(object sender, EventArgs e) {
            itemsReplaceE.Remove(CurrentReplaceE);
        }

        void SetListBoxReplaceE() { 
            string filter=textBoxReplaceEFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
           
            int index=listBoxReplaceE.SelectedIndex;
            listBoxReplaceE.Items.Clear();
            for (int i=0; i<itemsReplaceEFiltered.Count; i++) {
                ItemReplaceE item = itemsReplaceEFiltered[i];
                                
                string textToAdd=item.GetText();
                if (string.IsNullOrEmpty(textToAdd)) textToAdd="<Neznámé>";

                listBoxReplaceE.Items.Add(textToAdd);
            }

            if (index>=listBoxReplaceE.Items.Count)index=listBoxReplaceE.Items.Count-1;
            listBoxReplaceE.SelectedIndex=index;
        }

        void ReplaceERefreshFilteredList() {
            if (itemsReplaceEFiltered==null) itemsReplaceEFiltered=new List<ItemReplaceE>();
            itemsReplaceEFiltered.Clear();
            string filter=textBoxReplaceEFilter.Text;
            bool useFilter = filter!="" && filter!="*"; 
                  
            if (useFilter) {   
                for (int i=0; i<itemsReplaceE.Count; i++) {
                    ItemReplaceE item = itemsReplaceE[i];
                                    
                    if (item.Filter(filter)) { 
                        itemsReplaceEFiltered.Add(item);
                    }
                }
            } else { 
                for (int i=0; i<itemsReplaceE.Count; i++) {
                    ItemReplaceE item = itemsReplaceE[i];
                    itemsReplaceEFiltered.Add(item);
                }
            }
        }
               
        void AddNewItemReplaceE() {
            if (doingJob) return;
            doingJob=true;
            Edited=true;
            ChangeCaptionText();
            SaveCurrentReplaceE();

            var newItem=new ItemReplaceE();
           // newItem.ID=itemsPronouns.Count;
            itemsReplaceE.Add(newItem);
            CurrentReplaceE=newItem;
            ReplaceERefreshFilteredList();
            SetListBoxReplaceE(); 
            ListBoxSetCurrentReplaceE();
            SetCurrentReplaceE(); 
            
            doingJob=false;
        }
   
        void RemoveItemReplaceE(ItemReplaceE item) { 
            Edited=true;
            ChangeCaptionText();
            itemsReplaceE.Remove(item);
            ReplaceERefreshFilteredList();
            SetListBoxReplaceE();
            SetCurrentReplaceE();
        } 
           
        void SetCurrentReplaceE(){
            if (itemsReplaceEFiltered.Count==0) {
                SetNoneReplaceE();
                return;
            }

            int index=listBoxReplaceE.SelectedIndex;
            if (index>=itemsReplaceEFiltered.Count) index=itemsReplaceEFiltered.Count-1;
            if (index<0) index=0;
            CurrentReplaceE=itemsReplaceEFiltered[index];
            
            textBoxReplaceEFrom.Text=CurrentReplaceE.From;
            textBoxReplaceETo.Text=CurrentReplaceE.To;
        //    textBoxReplaceEFall.Text=CurrentReplaceE.Fall;

            textBoxReplaceEFrom.Visible=true;
            textBoxReplaceETo.Visible=true;
          //  textBoxReplaceEFall.Visible=true;

            labelReplaceEFrom.Visible=true;
            labelReplaceETo.Visible=true;
          //  labelReplaceEFall.Visible=true;
         
          //  ChangeTypeReplaceE(CurrentReplaceE?.Type);
        }
         
        void ListBoxSetCurrentReplaceE() {
            for (int indexCur=0; indexCur<itemsReplaceEFiltered.Count; indexCur++) {
                if (itemsReplaceE[indexCur]==CurrentReplaceE) { 
                    int indexList=listBoxReplaceE.SelectedIndex;
                    if (indexList==indexCur) return;
                    listBoxReplaceE.SelectedIndex=indexCur;
                    break;
                }
            }
        }  
        
        void SaveCurrentReplaceE() {
            if (CurrentReplaceE==null) return;

            CurrentReplaceE.From =textBoxReplaceEFrom.Text;
            CurrentReplaceE.To   =textBoxReplaceETo.Text;
         //   CurrentReplaceE.Fall =textBoxReplaceEFall.Text;
           
        } 
              
        void SetNoneReplaceE(){ 
            textBoxReplaceEFrom.Text="";
            textBoxReplaceETo.Text="";
          //  textBoxReplaceEFall.Text="";

            textBoxReplaceEFrom.Visible=false;
            textBoxReplaceETo.Visible=false;
         //   textBoxReplaceEFall.Visible=false;

            labelReplaceEFrom.Visible=false;
            labelReplaceETo.Visible=false;
        //    labelReplaceEFall.Visible=false;
        }
                
        void ClearReplaceE() { 
            listBoxReplaceE.Items.Clear();
            SetNoneReplaceE();
            itemsReplaceEFiltered?.Clear();
            itemsReplaceE?.Clear();
            CurrentReplaceE=null;
        }
        #endregion
    }
}