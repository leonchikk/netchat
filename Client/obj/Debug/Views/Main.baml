   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:Client.ConvertersClient.Converters  5   	   ;"clr-namespace:Client.Engine.ModelsClient.Engine.Models  5   	   clr-namespace:ClientClient  5   	       Client.Views.Main   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -cclr-namespace:Client.Converters     5   	   1md"clr-namespace:Client.Engine.Models     5   	   &localclr-namespace:Client     5   	   -   5          � WindowStartupLocation$  CenterScreen=�5
   	   $	��600��5   	   $	��800��6   $	��600��5   	   $	��800��6   .��5      �  � RowDefinitions 5   
   �� 5      $��40 �6   �� 5      $��40 �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��50 �6    � ColumnDefinitions 5   
   �� 5      $	��230 �6   �� 5      $��* �6   .+�5"   
   �� -   5         ��Name$ UserNameField��5"      $��0��5#      $��0��5$      $��Center=�5%      $��0,10,0,0q�5(   L   $��30��6^   ��5&      ��  �� Path$
 Name� �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�5*      � 5-   
   $��1��6   $��0��5.       50      �� 51      $��* �6#   �� 52      $	��100 �6#   �� 53      $��40 �6#   .+�56      �� -   5        $ InterlocutorNameField��56      $��0��57      $��Center=�58      $��30��5;   C   $��0,10,0,0q�6O   ��59      �� $ Interlocutor.Name�$ TwoWay=�$ PropertyChanged=���5<      �� 5=       �� 
TargetType" ��6    �� Triggers 5>      �� 5?      	 �� Binding	 6*   �� $ 
IsSelected�$ TwoWay=�$ PropertyChanged=�
 �� Value	
 False5B   &   .a�5D   "   ��  �� Property � y� �� Value$ Hidden=�6?   5E      5G      5I      �� 5J      -   5         ��Name$ RemoveContactButton��5J      $��2��5K      $��-��5M      ��5N      �� 5O      " ��6    5P      �� 5Q      	 6*   �� $ 
IsSelected�$ TwoWay=�$ PropertyChanged=�	
 False5T   &   .a�5U   "   ��  � y�$ Hidden=�6?   5V      �� 5W      	 6*   �� $ Interlocutor.IsFriend�$ TwoWay=�$ PropertyChanged=�	
 False5Z   &   .a�5[   "   ��  � y�$ Hidden=�6?   5\      5^      5`      �� 5b      -   5        $ StartVideoButton��5b      $��1��5c      $��Start video��5e      $
��10,0q�6+   ��5f      �� 5g      " ��6    5h      �� 5i      	 6*   �� $ 
IsSelected�$ TwoWay=�$ PropertyChanged=�	
 False5l   &   .a�5m   "   ��  � y�$ Hidden=�6?   5n      �� 5o      	 6*   �� $ Interlocutor.IsFriend�$ TwoWay=�$ PropertyChanged=�	
 False5r   &   .a�5s   "   ��  � y�$ Hidden=�6?   5t      �� 5u      	 6*   �� $ IsVideoCallStarted�$ TwoWay=�$ PropertyChanged=�
 True5x   &   .a�5y   "   ��  � y�$ Hidden=�6?   5z      5|      5~      �� 5      -   5        $ StopVideoButton��5      $��1��5�      $��
Stop video��5�      $
��10,0q�6*   ��5�      �� 5�      " ��6    5�      �� 5�      	 6*   �� $ 
IsSelected�$ TwoWay=�$ PropertyChanged=�	
 False5�   &   .a�5�   "   ��  � y�$ Hidden=�6?   5�      �� 5�      	 6*   �� $ Interlocutor.IsFriend�$ TwoWay=�$ PropertyChanged=�	
 False5�   &   .a�5�   "   ��  � y�$ Hidden=�6?   5�      �� 5�      	 6*   �� $ IsVideoCallStarted�$ TwoWay=�$ PropertyChanged=�	
 False5�   &   .a�5�   "   ��  � y�$ Hidden=�6?   5�      5�      5�      �� 5�      -   5        $ AddToContactButton��5�      $��2��5�      $��+��5�      ��5�      �� 5�      " ��6    5�      �� 5�      	 6*   �� $ 
IsSelected�$ TwoWay=�$ PropertyChanged=�	
 False5�   &   .a�5�   "   ��  � y�$ Hidden=�6?   5�      �� 5�      	 6*   �� $ Interlocutor.IsFriend�$ TwoWay=�$ PropertyChanged=�
 True5�   &   .a�5�   "   ��  � y�$ Hidden=�6?   5�      5�      5�      5�      � 5�   
   -   5         �Name$ MessageSection��5�      $��4��5�      $��1��5�       5�      �� 5�      $��* �6#   �� 5�      $��50 �6#   .+�5�      �� -	   5         ��Name$ MessageField��5�      $��0��5�      �� 5�      -
   5        $ SendMessageButton��5�      $��1��5�      $
��Send��5�      ��5�      �� 5�      " �6    5�      �� 5�      	 6&   �� $ 
IsSelected�$ TwoWay=�$ PropertyChanged=�	
 False5�   &   .a�5�      ��  � y�$ Hidden=�6;   5�      �� 5�      	 6&   �� $ Interlocutor.IsFriend�$ TwoWay=�$ PropertyChanged=�	
 False5�   &   .a�5�      ��  � y�$ Hidden=�6;   5�      �� 5�      	 6&   �� $ Interlocutor.IsApproved�$ TwoWay=�$ PropertyChanged=�	
 False5�   &   .a�5�      ��  � y�$ Hidden=�6;   5�      5�      5�      �� 5�   
   -   5         ��Name$ ConversationPanel��5�      $��1��5�      $��2��5�      $��2��5�      .+�5�      �� -   5         ��Name$ 
VideoField��5�      5�      �� 5�      -   5        $ MessagePanel��5�      .+�5      �� -   5        $ ApproveContactButton��5      $��Approve contact��5     ��5     �� 5     " ��6     5     �� 5  "   	 6.   �� $ Interlocutor.IsApproved�$ TwoWay=�$ PropertyChanged=�
 True5	  &   .a�5
  &   ��  � y�$ 	Collapsed=�6C   5  #   �� 5  "   	 6.   �� $' !Interlocutor.IsInitiatorToApprove�$ TwoWay=�$ PropertyChanged=�	
 False5  &   .a�5  &   ��  � y�$ 	Collapsed=�6C   5  #   5     5     �� 5     -   5         ��Name$ MessagesList��5     ��5     �� 5     " ��6     5     �� 5  "   	 6.   �� $ Interlocutor.IsApproved�$ TwoWay=�$ PropertyChanged=�	
 False5  &   .a�5   &   ��  � y�$ Hidden=�6C   5!  #   5#     5%     ��5'     �� 5(     " ��6    5)     �� 5*     	 6*   �� $ 
IsSelected�$ TwoWay=�$ PropertyChanged=�	
 False5-  &   .a�5/  "   ��  � y�$ Hidden=�6?   50     �� 51     	 6*   �� $ Interlocutor.IsFriend�$ TwoWay=�$ PropertyChanged=�	
 False54  &   .a�55  "   ��  � y�$ Hidden=�6?   56     58     5:     5<     �� 5>  
   -   5        $ SearchField��5>     $��0��5?     $��1��5@     $��0 5 0 5q�5C     �� 5F  
   -   5        $ ContactList��5F     $��0��5H     $��2��5I     $��3��5J     ��5K     �� 5L     .R�5M     � .+�5N     �� ��6    �� *Name+5O     5P     ��5S     �� 5T     " ��6    5U     �� 5V     	 6&   �� *IsSearchMode+
 True6G   .a�5W     ��  � y�$ 	Collapsed=�6;   5X     5Z     5\     �� 5_  
   -   5        $ SearchResultsList��5_     $��0��5a     $��2��5b     $��4��5c     ��5d     �� 5e     .R�5f     � .+�5g     �� ��6    �� *Name+5h     5i     ��5l     �� 5m     " ��6    5n     �� 5o     	 6&   �� *IsSearchMode+	
 False6G   .a�5p     ��  � y�$ 	Collapsed=�6;   5q     5s     5u     5w     5x     