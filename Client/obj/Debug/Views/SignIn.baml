   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient  5   	       Client.Views.SignIn   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:Client     5   	   -   5          � WindowStartupLocation$  CenterScreen=�5   	   $	��370��5   	   $	��500��6   $	��370��5   	   $	��500��6   .��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $	��250 �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $	��440 �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��0��5      $��3��5      $��3��5      ��5      �� 5       �� 
StartPoint$
 0, 0&�6&    �� EndPoint$
 0, 1&�68   .��5      �  � Color$ 	#FF79EDFF��6#    � Offset$ 0Q�65   � 5       $ Transparent��6#   $	 1.0Q�67   5!      5#      �� 5%   
   $��1��6   $��1��6%   $��20q�5&      .+�5(      ��  �� Orientation$ 
Horizontal=�6   $��Right=�62   $��	0 20 50 0q�5)      .+�5*      �� $	��Top=�5+      ���	#FF464242$��18(�5-      $��0 0 10 0q�5.      $��Email:��5/      �� 50      -   5         ��Name$ 
EmailField��54      $��Center=�51      $��0 3 0 3q�52      $	��250��53      54      55      �� 57      $ 
Horizontal=�6   $��Right=�58      $��	0 20 50 0q�59      .+�5:      �� ���	#FF464242$	��Top=�5<      $��18(�5=      $��0 0 10 0q�5>      $��	Password:��5?      G� 5A      -   5        	 G�Name$	 PasswordField��5D      $	��250��5A      $��0 3 0 3q�5B      $��Center=�5C      5D      5F      �� 5H      -   5        
 ��Name$
 
ErrorField��5H      $��12(�5I      
���  ��$��Center=�5K      $��	70 5 70 0q�5L      $�� ��5M      $y�Hidden=�5N      �� 5P      -   5         ��Name$ 	SignInBtn��5P      $��Sign In��5Q      $��130 5 130 5q�5S      $��25��5T      �� 5V      -   5        $
 	SignUpBtn��5V      $��Center=�5W      $'��!or Sign Up if you haven't account��5X      $��100 0q�5Z      5[      5]      5_      