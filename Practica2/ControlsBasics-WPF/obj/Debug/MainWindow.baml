   M S B A M L   `   `   `  ����M  IControlsBasics-WPF, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:Microsoft.Samples.Kinect.ControlsBasics'Microsoft.Samples.Kinect.ControlsBasics  5   	   8    2Microsoft.Samples.Kinect.ControlsBasics.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   S OMicrosoft.Kinect.Toolkit, Version=1.8.0.0, Culture=neutral, PublicKeyToken=null\ XMicrosoft.Kinect.Toolkit.Controls, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null2k(http://schemas.microsoft.com/kinect/2013   5   	   <d2http://schemas.microsoft.com/expression/blend/2008   5   	   Fmc;http://schemas.openxmlformats.org/markup-compatibility/2006   5   	   Clocal5clr-namespace:Microsoft.Samples.Kinect.ControlsBasics   5   	   -   5        $��ControlsBasicsWindow��5   	     � Title$  Controls Basics��5	   	   $	��500��5   	   $	��700��5   	   $��Segoe UI)�5   	    � 	Resources 5         ChevronGeometry  	ChevronUp �� Stretch
 �� Top �� FallbackValue �� Path �� RelativeSource �� AncestorType?  9Microsoft.Samples.Kinect.ControlsBasics.KinectHoverButton J� RenderTransformOrigin	 �� Angle  ChevronRight  ChevronDown  ChevronLeft  KinectHoverButtonStyle
 �� 
TargetType �� Property �� Value �� 
TargetType �� Triggers P� Property P� Value " KinectHoverButtonScrollUpStyle �� BasedOn % !KinectHoverButtonScrollRightStyle $  KinectHoverButtonScrollDownStyle $	  KinectHoverButtonScrollLeftStyle��%�  &	       &	 �    7   &	 Y   7   &	 .   7   &	    7   &	 �    &	 �    7  7  &	 ?    7  7  &	 �    7  7  &		 �    7  7  B�5   
   miF1M150.001,146.9932L126.565,169.3822L126.565,153.5962L150.001,130.6182L173.435,153.5962L173.435,169.3822z�5      F� 5   
   $��48��6!   $��48��6-   $ Uniform=�68   $ 0��6J   8��  ��5      �� $ Black��$ 
Foreground� �� "  $ 0.5,0.5&�5   M   z�5      �� 5      $	 0Q�6"   5      F� 5!   
   $��48��6$   $��48��60   $ Uniform=�6;   $ 0��6M   8��  ��5#      �� $ Black��$ 
Foreground� �� "  $ 0.5,0.5&�5$   M   z�5%      �� 5&      $	 90Q�6"   5(      F� 5*   
   $��48��6#   $��48��6/   $ Uniform=�6:   $ 0��6L   8��  ��5,      �� $ Black��$ 
Foreground� �� "  $ 0.5,0.5&�5-   M   z�5.      �� 5/      $		 180Q�6"   51      F� 53   
   $��48��6#   $��48��6/   $ Uniform=�6:   $ 0��6L   8��  ��55      �� $ Black��$ 
Foreground� �� "  $ 0.5,0.5&�56   M   z�57      �� 58      $		 270Q�6"   5:      �� 5<   
   "
  6/   .�5=      ��  � ��
 �����6+   �� 5>       � ��
 �����6,   �� 5?       � ��$ 0q�60   6<   �� 5@       � ��
 �����6;   �� 5A       � �� 5B      �� 5C      "  6&   .R�5D      � $��Stretch=�6   $��Stretch=�6=   #��z��6Y   #��z��6�   #��z��6�   .+�5E      �� #��z��6%   #��z��6Y   ._�5F   "   �� $��Stretch=�63   $��Stretch=�6Q   #��z��6m   6�   5G      5H      5I      5K       5L      P� 5M       � {� .  . �5N      ��  � ��
 �����63   �� 5O       � ��
 �����64   �� 5P       � ��
 ���� 6I   5Q      P� 5R       �@ IsHandPointerOver . . �5S      ��  � �� �#5A5B5B63   �� 5T       � ��
 �����63   5U      5W      �� 5Y   
   "
  67   8   .�5Z      ��  � ��8  6(   5[      �� 5]   
   "
  6:   8   .�5^      ��  � ��8  6(   5_      �� 5a   
   "
  69   8   .�5b      ��  � ��8  6(   5c      �� 5e   
   "
  69   8   .�5f      ��  � ��8  6(   5g      5h      .��5j      4  .Microsoft.Kinect.Toolkit.Controls.KinectRegion  -   5          Name$ kinectRegion��5j      .��5k   
   � -   5         �Name$ kinectRegionGrid��5k      .+�5l      �� -   5        $��fondo��5l      $y�Hidden=�6!    J� Opacity$	 0.3Q�65   8  2Microsoft.Kinect.Toolkit.Controls.KinectUserViewer  5m      $	��100��6b   $��Right=�6o   $	��Top=�6�      KinectRegion 6!   ��  �� ElementName$ kinectRegion��6�   :  4Microsoft.Kinect.Toolkit.Controls.KinectScrollViewer  5n      -   5        $��scrollViewer��5n   #   $��Visible=�67   $��Disabled=�6_   $	��200��6�   $��	0,0,150,0q�6�   $
��Left=�6�   $	��Top=�6�   .��5o      � -   5         �Name$ 	wrapPanel��5o      $	��Top=�6/    � Orientation$ 
Horizontal=�6H   .+�5q      8  2Microsoft.Kinect.Toolkit.Controls.KinectTileButton     Label$ 1��6)     5r      $ 2��6)   5s      5t        5v      -   5        $��scrollViewer2��5v   #   $��Disabled=�68   $��Disabled=�6a   $��Right=�6�   $��Bottom=�6�   $��0,150,150,50q�6�   .��5w      � -   5        $ 
wrapPanel2��5w   7   $��Center=�6   $ Vertical=�6K   5y      5z      �� 5|      -	   5        $��Imagen_pregunta��5|      $
��Left=�6+   $	��500��6F   $
��1000��6S   $��50,0,0,5q�6`   $��Bottom=�6r   $y�Visible=�6�     5}      -
   5        $ Pregunta��5}   !   $
��Left=�62   $	��500��6M   $
��1000��6Z   $��50,0,0,5q�6g   $��Bottom=�6y   $ 0Q�6�   �� 5      -   5        $��correcto��5      $	��140��6%   $	��140��62   $��Right=�6?   $��Bottom=�6\   $��	0,0,0,175q�6w   $y�Hidden=�6�   �� 5�      -   5         ��Name$ ContadorResultados��5�      $��Right=�64   $��Bottom=�6P   $��
0,0,40,350q�6k   $��30(�6   $��60��6�   �� 6�   $	�0/5��6�   6�   5�      5�      5�      