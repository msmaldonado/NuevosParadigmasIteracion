   M S B A M L   `   `   `  ����  SkeletonBasics-WPF8    2Microsoft.Samples.Kinect.SkeletonBasics.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  Skeleton Basics��5   	   $	��809��6!   $	��770��6.    � 	Resources 5         MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush �� 
TargetType �� Property �� Value  SquareCheckBox �� 
TargetType �� Orientation �Name	 ��Name
 ��Name �� TextAlignment �� Triggers P� Property P� Value �� 
TargetName��%1  &	        &	 #     &	 F     '�� i     &	 �     7  7  �� 5   
   $ 	#ff6e6e6e��62   �� 5   
   $ 	#ff52318f��64   �� 5	   
   $ 	#ff00BCF2��62   �� 5
   
   " ��6   .�5      ��  �@J�SnapsToDevicePixels . 64   5      �� 5   
   " ��6   .�5      ��  � �� 5      �� 5      " ��6&   .R�5      � .+�5      �� $ 
Horizontal=�6)   
������ .+�5   "   � $ SquareCheckBoxChecked��6'   .+�5   &   �� $	 CheckedNormal��6,   $��Images\CheckedNormal.png��6C   $
��None=�6e   $��Center=�6t   �� 5   &   $	 CheckedHover��6,   $��Images\CheckedHover.png��6B   $
��None=�6c   $��Center=�6r   $y�	Collapsed=�6�   5   #   � 5   "   $ SquareCheckBoxUnchecked��6'   $y�	Collapsed=�6H   .+�5   &   �� $	 UncheckedNormal��6,   $ ��Images\UncheckedNormal.png��6E   $
��None=�6i   $��Center=�6x   �� 5   &   $	 UncheckedHover��6,   $��Images\UncheckedHover.png��6D   $
��None=�6g   $��Center=�6v   $y�	Collapsed=�6�   5   #   �� 5   "   $
 SquareCheckBoxText��6,   #��z��6H   $
 Left=�6i   $��Center=�6   8��  $��15(�6�   $��9,0,0,0q�6�   5      5       5      P� 5       �@d�	IsChecked$ falseY�6;   . �5    "   �� $ SquareCheckBoxChecked��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5!   "   $ SquareCheckBoxUnchecked��6O    � y�$ Visible=�6?   6O   5"      P� 5#       �@J�IsMouseOver . . �5$   "   �� $ CheckedNormal��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5%   "   $ UncheckedNormal��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5&   "   $ CheckedHover��6O    � y�$ Visible=�6?   6O   �� 5'   "   $ UncheckedHover��6O    � y�$ Visible=�6?   6O   �� 5(   "   8  $ SquareCheckBoxText��6h    � ��5)      5+      5-      5.      5/      .��51      � -   5        $��
layoutGrid��51      $��	10 0 10 0q�6    � ColumnDefinitions 52   
   �� 53      $
��135* �6   �� 54      $
��608* �6    � RowDefinitions 56   
   �� 57      $
��Auto �6   �� 58      $��* �6   �� 59      $
��Auto �6   �� 5:      $
��Auto �6   .+�5<   
   ]� $��0��6   $��0,0,0.2,20.2q�6!   $��2��67   .+�5=      �� $
��Left=�6   $��Images\logo.png��6*   $
��Fill=�6C   $��32��6R   $��81��6^   $��0 10 0 5q�6i   �� 5>      $��Right=�6   $��0 0 -1 0q�6/   $��Bottom=�6A   #��[  6\   $��Segoe UI)�6�   $��18(�6�   P1_NPI6�   �� 5?      $��1��6   $��Images\Status.png��6$   $
��None=�6?   $��Center=�6N   $��0 0 0 0q�6k   5@      (� 5A   
    (� Stretch$ Uniform=�6   $��Center=�6$   $��0,0,0.2,49.2q�6A   $��4��6W   $��2��6h   .��5B      �� -   5        $��ColorI��5B      $	��640��6"   $	��480��6.   5C      (� 5D   
   $ Uniform=�6   $��Center=�6$   $��0,0,0.2,49.2q�6A   $��4��6W   $��2��6h   .��5E      �� -   5        $��	Esqueleto��5E      $	��640��6%   $	��480��61   5F      �� 5G   
   -   5        $��
statusBar1��5G   ?   $��3��6   $��center=�6!   $��Bottom=�6Q   
�������#��[ 6   $��50��6�   $��2��6�   $��0,0,10,0q�6�   $	��732��6�   .@�5H      �� $��Bottom=�6   $
��left=�67   �� 5I      -   5        $
 FeedbackTexBlock��5I      $��0,0,0,0q�62   $��30(�6C   $	��722��6Q   �� 6^   $ �Comienzan los movimientos!��6b   6�   5J      ]� 5L   
   $��2��6   $��0,0,0,0q�6!   $��2��62   .+�5M      �� $
��Left=�6   $��Images\inicial.png��6*   $
��Fill=�6F   $	��300��6U   $	��150��6b   $��0 10 0 5q�6n   �� 5N      $��Bottom=�6   $��0 0 -1 0q�60   $��Bottom=�6B   $��25(�6]   	inicial6t   5O      ]� 5Q   
   $��3��6   $��0,0,0,0q�6!   $��3��62   .+�5R      �� $
��Left=�6   $��Images\final.png��6*   $
��Fill=�6D   $	��300��6S   $	��150��6`   $��0 10 0 5q�6l   �� 5S      $��Bottom=�6   $��0 0 -1 0q�60   $��Bottom=�6B   $��25(�6]   final6r   5T      5U      5V      