   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient  5   	       Client.Views.SignUp   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:Client     5   	   -   5          � WindowStartupLocation$  CenterScreen=�5   	   $	��350��5   	   $	��600��6   $	��350��5   	   $	��600��6   .��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $	��300 �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $	��440 �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��0��5      $��3��5      $��3��5      ��5      �� 5       �� 
StartPoint$
 0, 0&�6&    �� EndPoint$
 0, 1&�68   .��5      �  � Color$ 	#FF79EDFF��6#    � Offset$ 0Q�65   � 5       $ Transparent��6#   $	 1.0Q�67   5!      5#      �� 5%   
   $��1��6   $��1��5&      $��20q�5'      .+�5)      ��  �� Orientation$ 
Horizontal=�6   $��Right=�62   $��	0 20 50 0q�5*      .+�5+      �� $	��Top=�5,      ���	#FF464242$��18(�5.      $��0 0 10 0q�5/      $��Name:��50      �� 51      -   5         ��Name$ 	NameField��55      $��Center=�52      $��0 3 0 3q�53      $	��250��54      55      56      �� 58      $ 
Horizontal=�6   $��Right=�62   $��	0 20 50 0q�59      .+�5:      �� $	��Top=�5;      ���	#FF464242$��18(�5=      $��0 0 10 0q�5>      $��Email:��5?      �� 5@      -   5        $ 
EmailField��5D      $��Center=�5A      $��0 3 0 3q�5B      $	��250��5C      5D      5E      �� 5G      $ 
Horizontal=�6   $��Right=�5H      $��	0 20 50 0q�5I      .+�5J      �� ���	#FF464242$	��Top=�5L      $��18(�5M      $��0 0 10 0q�5N      $��	Password:��5O      G� 5Q      -   5        	 G�Name$	 PasswordField��5T      $	��250��5Q      $��0 3 0 3q�5R      $��Center=�5S      5T      5V      �� 5X      -   5        
 ��Name$
 
ErrorField��5X      
���  ��$�� ��5Z      $��Center=�5[      $y�Hidden=�5\      �� 5^      -   5         ��Name$ 	SignUpBtn��5^      $��Sign Up��5_      $��130 15 130 5q�5`      $��25��5a      5b      �� 5d      -   5        $
 	SignInBtn��5d      $��Center=�5e      $$��or Sign Ip if you have account��5f      $��100 0q�5g      5h      5i      5k      5m      